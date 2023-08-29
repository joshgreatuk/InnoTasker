using Discord;
using Discord.Interactions;
using Discord.WebSocket;
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

        private readonly IServiceProvider _services;
        private readonly ILogger _logger;
        private readonly DiscordSocketClient _client;
        private readonly InteractionService _interactionService;

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
            if (Program.IsDebug())
            {
                await _interactionService.AddCommandsToGuildAsync(s_testGuild, true);
            }
            else
            {
                await _interactionService.AddCommandsGloballyAsync(true);
            }
            await _logger.LogAsync(LogSeverity.Info, this, $"InteractionHandler Initialized!");
        }

        public async Task OnInteraction(SocketInteraction interaction)
        {
            try
            {
                SocketInteractionContext context = new(_client, interaction);
                await _interactionService.ExecuteCommandAsync(context, _services);
            }
            catch (Exception ex)
            {
                await _logger.LogAsync(LogSeverity.Error, this, "OnInteraction Error", ex);
            }
        }

        public async Task OnInteractionExecuted(ICommandInfo info, IInteractionContext context, IResult result)
        {
            if (result.IsSuccess) return;

            await _logger.LogAsync(LogSeverity.Error, this, "OnInteractionExecuted Error", new Exception(result.ErrorReason));
            await context.Interaction.RespondAsync(embed: new EmbedBuilder().WithTitle($"InnoTasker Error").WithDescription(result.ErrorReason).Build());
        }
    }
}
