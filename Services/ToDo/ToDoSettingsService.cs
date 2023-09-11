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
        private readonly IGuildService _guildService;
        private readonly IToDoListService _toDoListService;

        private readonly ISettingsPageBuilder toDoListMenuBuilder;
        private readonly List<ISettingsPageBuilder> settingsPages;
        private List<ToDoSettingsInstance> toDoSettingsInstances = new();

        public ToDoSettingsService(ILogger logger, IGuildService guildService, IToDoListService toDoListService) : base(logger)
        {
            _guildService = guildService;
            _toDoListService = toDoListService;

            //Add settings pages
            toDoListMenuBuilder = new ToDoListMenuBuilder(_guildService, this);
            settingsPages = new()
            {
                new ToDoSettingsChannelsBuilder(),
                new ToDoSettingsCategoriesBuilder(),
                new ToDoSettingsPermissionsBuilder()
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

                if (instance.message != null)
                {
                    await UpdateInstance(interaction, message);
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

        public async Task<bool> OpenSettings(SocketInteraction interaction, string toDoListName, ToDoSettingsContext context)
        {
            ToDoSettingsInstance instance = await GetSettingsInstance(interaction.Channel.Id);
            try
            {
                instance.guildID = (ulong)interaction.GuildId;
                instance.context = context;
                instance.pageIndex = 0;
                instance.toDoListName = toDoListName;
                instance.mode = ToDoSettingsInstanceMode.ToDoSettings;

                if (context is not ToDoSettingsContext.New) await instance.GetListData(_guildService);

                MessageContext message = await GetSettingsPage(instance);

                if (instance.message != null)
                {
                    await UpdateInstance(interaction, message);
                }
                else
                {
                    instance.message = await interaction.Channel.SendMessageAsync(embed: message.embed, components: message.component.Build());
                }
            }
            catch (Exception ex)
            {
                _logger.Log(LogSeverity.Error, this, $"Interaction {interaction.Channel.Id} failed to open ToDoListPage", ex);
                await CloseInstance(instance, "There was an error opening this list's settings page");
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
            page.component.WithButton("<-", "settings-last", ButtonStyle.Secondary);
            if (await settingsPages[instance.pageIndex].CanProceed(instance))
            {
                page.component.WithButton("Save & Close", "settings-close", ButtonStyle.Danger);
            }
            else
            {
                page.component.WithButton("Cancel", "settings-list-close", ButtonStyle.Danger);
            }
            page.component.WithButton("->", "settings-next", ButtonStyle.Secondary, disabled: !await settingsPages[instance.pageIndex].CanProceed(instance));
            return page;
        }
        public async Task<MessageContext> GetCurrentSettingsPage(ulong interactionID) => await GetSettingsPage(await GetSettingsInstance(interactionID));

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
                MessageContext context = (MessageContext)message;
                await UpdateInstance(interaction, context);
            }
        }

        public async Task<bool> UpdateInstance(SocketInteraction interaction, MessageContext context)
        {
            try
            {
                ToDoSettingsInstance instance = await GetSettingsInstance(interaction.Channel.Id);
                if (instance.message != null)
                {
                    await instance.message.ModifyAsync(x => { x.Embed = context.embed; x.Components = context.component.Build(); });
                }
                else
                {
                    await interaction.Channel.SendMessageAsync(embed: context.embed, components: context.component.Build());
                }
            }
            catch (Exception ex)
            {
                await _logger.LogAsync(LogSeverity.Error, this, $"Failed to update instance {interaction.Channel.Id}", ex);
                await CloseInstance(interaction.Channel.Id, "There was an error updating this instance");
                return false;
            }
            return true;
        }

        public async Task<bool> InstanceExists(ulong instanceID) => toDoSettingsInstances.Exists(x => x.interactionID == instanceID);

        public async Task<string> GetCurrentInstanceListName(ulong instanceID) => GetSettingsInstance(instanceID).Result.toDoListName;

        public async Task<bool> SaveInstance(ulong instanceID)
        {
            ToDoSettingsInstance instance = await GetSettingsInstance(instanceID);
            try
            {
                await instance.SaveGuildSettings(_guildService);
            }
            catch (Exception ex)
            {
                await _logger.LogAsync(LogSeverity.Error, this, $"Settings instance {instanceID} failed to save", ex);
                await CloseInstance(instanceID, "There was an error saving this instance");
                return false;
            }
            return true;
        }

        public async Task Shutdown() 
        {
            //Close all settings instances and leave a message saying sorry :P
            foreach (ToDoSettingsInstance instance in toDoSettingsInstances.ToList())
            {
                await CloseInstance(instance, "Closed for bot maintenence. Sorry for the inconvenience <3");
            }
            toDoSettingsInstances.Clear();
            await _logger.LogAsync(LogSeverity.Info, this, $"All ToDoSettingsInstances closed");
        }

        public async Task CloseInstance(ulong interactionID, string? message=null) => await CloseInstance(await GetSettingsInstance(interactionID), message);

        public async Task CloseInstance(ToDoSettingsInstance instance, string? message=null)
        {
            if (message != null && instance.message != null)
            {
                //Instead of removing instance message, replace it with a sorry message
                EmbedBuilder newEmbed = new EmbedBuilder().WithTitle("Sorry!")
                    .WithDescription(message);
                await instance.message.ModifyAsync(x => { x.Embed = newEmbed.Build(); x.Components = null; });
                await _logger.LogAsync(LogSeverity.Debug, this, $"Closed instance {instance.interactionID} with message specified");
            }
            else if (instance.message != null)
            {
                await instance.message.DeleteAsync();
                await _logger.LogAsync(LogSeverity.Debug, this, $"Closed settings instance {instance.interactionID}");
            }
            await _guildService.SaveGuild(instance.guildID);

            toDoSettingsInstances.Remove(instance);
        }
    }
}
