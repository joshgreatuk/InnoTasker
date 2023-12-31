﻿using Discord;
using Discord.Interactions;
using Discord.Rest;
using Discord.WebSocket;
using InnoTasker.Modules;
using InnoTasker.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace InnoTasker
{
    public class InteractionHandler
    {
        private static readonly ulong s_testGuild = 1096556224167825408;
        private static readonly ulong s_adminGuild = 1096556224167825408;

        private static readonly TimeSpan s_userCooldown = TimeSpan.FromSeconds(3);

        private readonly IServiceProvider _services;
        private readonly ILogger _logger;
        private readonly DiscordSocketClient _client;
        private readonly InteractionService _interactionService;

        private Dictionary<ulong, DateTime> userCooldowns = new();

        public InteractionHandler(IServiceProvider services)
        {
            _services = services;
            _logger = services.GetRequiredService<ILogger>();

            _client = services.GetRequiredService<DiscordSocketClient>();
            _client.Ready += OnClientReady;
            _client.InteractionCreated += OnInteraction;

            _interactionService = services.GetRequiredService<InteractionService>();
            _interactionService.Log += _logger.LogAsync;
            _interactionService.InteractionExecuted += OnInteractionExecuted;
        }

        public async Task OnClientReady()
        {
            await _interactionService.AddModulesAsync(Assembly.GetEntryAssembly(), _services);

            if (InnoTasker.IsDebug())
            {
                await _interactionService.AddModulesToGuildAsync(s_testGuild, true, _interactionService.Modules.ToArray());
            }
            else
            {
                await _interactionService.AddModulesGloballyAsync(true, _interactionService.Modules.Where(x => x.Name != "BotAdminModule").ToArray());
                await _interactionService.AddModulesToGuildAsync(s_adminGuild, true, _interactionService.GetModuleInfo<BotAdminModule>());
            }

            await _logger.LogAsync(LogSeverity.Info, this, $"InteractionHandler Initialized!");
        }

        public async Task OnInteraction(SocketInteraction interaction)
        {
            if (interaction.Type is not InteractionType.ApplicationCommand) return;

            if (userCooldowns.TryGetValue(interaction.User.Id, out DateTime cooldownTime))
            {
                if (cooldownTime.Add(s_userCooldown) > DateTime.UtcNow)
                {
                    await interaction.RespondAsync(embed: new EmbedBuilder()
                        .WithTitle("Woah there!")
                        .WithDescription("Sorry, there is a 3 second cooldown on commands per user")
                        .WithCurrentTimestamp()
                        .Build(), ephemeral: true);
                    return;
                }
                userCooldowns.Remove(interaction.User.Id);
            }
            userCooldowns.Add(interaction.User.Id, DateTime.UtcNow);

            try
            {
                SocketInteractionContext context = new(_client, interaction);
                IResult result = await _interactionService.ExecuteCommandAsync(context, _services);
            }
            catch (Exception ex)
            {
                await _logger.LogAsync(LogSeverity.Error, this, "OnInteraction Error", ex);
            }
        }

        public async Task OnInteractionExecuted(ICommandInfo info, IInteractionContext context, IResult result)
        {
            if (result.IsSuccess) return;

            if (result.Error is InteractionCommandError.UnmetPrecondition)
            {
                await context.Interaction.RespondAsync(result.ErrorReason, ephemeral: true);
            }
            else
            {
                await _logger.LogAsync(LogSeverity.Error, this, result.ErrorReason);
            }
        }
    }
}
