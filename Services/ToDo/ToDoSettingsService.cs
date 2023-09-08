using Discord;
using Discord.WebSocket;
using InnoTasker.Data;
using InnoTasker.Modules.Settings;
using InnoTasker.Services.Interfaces;
using InnoTasker.Services.Interfaces.ToDo;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnoTasker.Services.ToDo
{
    public class ToDoSettingsService : InnoServiceBase, IToDoSettingsService
    {
        private readonly DiscordSocketClient _client;
        private readonly IGuildService _guildService;
        private readonly IToDoListService _toDoListService;

        ISettingsPageBuilder toDoListMenuBuilder;
        public List<ISettingsPageBuilder> settingsPages;
        public List<ToDoSettingsInstance> toDoSettingsInstances = new();

        public ToDoSettingsService(ILogger logger, DiscordSocketClient client, IGuildService guildService, IToDoListService toDoListService) : base(logger)
        {
            _client = client;
            _guildService = guildService;
            _toDoListService = toDoListService;

            //Add settings pages
            toDoListMenuBuilder = new ToDoListMenuBuilder(_guildService, this);
            settingsPages = new()
            {

            };
        }

        public async Task<bool> OpenToDoListPage(SocketInteraction interaction) //Settings page should replace this menu, so should create an instance?
        {
            ToDoSettingsInstance instance = await GetSettingsInstance(interaction.Channel.Id);
            try
            {
                instance.guildID = (ulong)interaction.GuildId;
                instance.mode = ToDoSettingsInstanceMode.ToDoMenu;

                MessageContext message = await toDoListMenuBuilder.BuildPage(instance);
                message.component.WithButton("Close", "settings-close");

                if (instance.message != null)
                {
                    await UpdateInstance(interaction.Channel.Id, message);   
                }
                else
                {
                    //Send the message
                    instance.message = await interaction.Channel.SendMessageAsync(embed: message.embed, components: message.component.Build());
                }
            }
            catch (Exception ex)
            {
                _logger.Log(LogSeverity.Error, this, $"Interaction {interaction.Channel.Id} failed to open ToDoListPage", ex);
                await CloseInstance(instance, "Error, sorry");
                return false;
            }
            return true;
        }

        public async Task<bool> OpenSettings(SocketInteraction interaction, string toDoListName)
        {
            ToDoSettingsInstance instance = await GetSettingsInstance(interaction.Channel.Id);
            try
            {
                instance.guildID = (ulong)interaction.GuildId;
                instance.pageIndex = 0;
                instance.toDoListName = toDoListName;
                instance.mode = ToDoSettingsInstanceMode.ToDoSettings;

                MessageContext message = await GetSettingsPage(instance);

                if (instance.message != null)
                {
                    await UpdateInstance(instance.interactionID, message);
                }
                else
                {
                    instance.message = await interaction.Channel.SendMessageAsync(embed: message.embed, components: message.component.Build());
                }
            }
            catch (Exception ex)
            {
                _logger.Log(LogSeverity.Error, this, $"Interaction {interaction.Channel.Id} failed to open ToDoListPage", ex);
                await CloseInstance(instance, "Error, sorry");
                return false;
            }
            return true;
        }

        public async Task<MessageContext> GetSettingsPage(ulong interactionID, int index)
        {
            ToDoSettingsInstance instance = await GetSettingsInstance(interactionID);
            instance.pageIndex = index;
            return await GetSettingsPage(instance);
        }

        public async Task<MessageContext> GetSettingsPage(ToDoSettingsInstance instance)
        {
            //Add back, close and forward buttons before sending
            ISettingsPageBuilder builder = settingsPages[instance.pageIndex];
            MessageContext page = await builder.BuildPage(instance);
            page.component.WithButton("<-", "settings-last", ButtonStyle.Secondary)
                .WithButton("Close", "settings-close", ButtonStyle.Danger)
                .WithButton("->", "settings-next", ButtonStyle.Secondary);
            return page;
        }

        public async Task<MessageContext> GetNextSettingsPage(ulong interactionID)
        {
            ToDoSettingsInstance instance = await GetSettingsInstance(interactionID);
            if (instance.pageIndex == settingsPages.Count - 1) return await GetSettingsPage(instance);
            instance.pageIndex++;
            return await GetSettingsPage(instance);
        }

        public async Task<MessageContext> GetLastSettingsPage(ulong interactionID)
        {
            ToDoSettingsInstance instance = await GetSettingsInstance(interactionID);
            if (instance.pageIndex == 0) return await GetSettingsPage(instance);
            instance.pageIndex--;
            return await GetSettingsPage(instance);
        }

        public async Task<ToDoSettingsInstance> GetSettingsInstance(ulong interactionID)
        {
            ToDoSettingsInstance? instance = toDoSettingsInstances.FirstOrDefault(x => x.interactionID == interactionID);
            if (instance == null)
            {
                instance = new ToDoSettingsInstance(interactionID);
                toDoSettingsInstances.Add(instance);
            }
            return instance;
        }

        public async Task HandleInteraction(SocketInteraction interaction)
        {
            ToDoSettingsInstance instance = await GetSettingsInstance(interaction.Channel.Id);
            MessageContext? message = null;
            switch (instance.mode)
            {
                case ToDoSettingsInstanceMode.ToDoMenu:
                    {
                        message = await toDoListMenuBuilder.HandleInteraction(instance, interaction);
                        break;
                    }
                case ToDoSettingsInstanceMode.ToDoSettings:
                    {
                        message = await settingsPages[instance.pageIndex].HandleInteraction(instance, interaction);
                        break;
                    }
            }

            if (message != null)
            {
                await UpdateInstance(interaction.Channel.Id, (MessageContext)message);
            }
        }

        public async Task<bool> UpdateInstance(ulong interactionID, MessageContext context)
        {
            return false;
        }

        public async void Shutdown() 
        {
            //Close all settings instances and leave a message saying sorry :P
            foreach (ToDoSettingsInstance instance in toDoSettingsInstances.ToList())
            {
                await CloseInstance(instance, "Closed for bot maintenence. Sorry for the inconvenience <3");
            }
            toDoSettingsInstances.Clear();
            await _logger.LogAsync(LogSeverity.Info, this, $"All ToDoSettingsInstances closed");
        }

        public async Task CloseInstance(ulong interactionID) => CloseInstance(await GetSettingsInstance(interactionID));

        public async Task CloseInstance(ToDoSettingsInstance instance, string? message=null)
        {
            if (message != null && instance.message != null)
            {
                //Instead of removing instance message, replace it with a sorry message
                EmbedBuilder newEmbed = new EmbedBuilder().WithTitle("Sorry!")
                    .WithDescription(message);
                await _logger.LogAsync(LogSeverity.Debug, this, $"Closed instance {instance.interactionID} with message specified");
            }
            else if (instance.message != null)
            {
                await instance.message.DeleteAsync();
                await _logger.LogAsync(LogSeverity.Debug, this, $"Closed settings instance {instance.interactionID}");
            }
            _guildService.SaveGuild(instance.guildID);

            toDoSettingsInstances.Remove(instance);
        }
    }
}
