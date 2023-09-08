using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using InnoTasker.Data;
using InnoTasker.Services.Interfaces;
using InnoTasker.Services.Interfaces.ToDo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnoTasker.Modules.Settings
{
    public class ToDoListMenuBuilder : ISettingsPageBuilder
    {
        private readonly IGuildService _guildService;
        private readonly IToDoSettingsService _settingsService;

        public ToDoListMenuBuilder(IGuildService guildService, IToDoSettingsService settingsService)
        {
            _guildService = guildService;
            _settingsService = settingsService;
        }

        public async Task<MessageContext> BuildPage(ToDoSettingsInstance instance)
        {
            GuildData guild = _guildService.GetGuildData(instance.guildID);
            List<string> toDoListEntries = guild.Lists.Select(x => x.Name).ToList();
            string toDoListDescription = String.Join('\n', toDoListEntries);
            if (instance.toDoListName == String.Empty &&  toDoListEntries.Count > 0) instance.toDoListName = toDoListEntries[0];
            EmbedBuilder embed = new EmbedBuilder().WithTitle("To-Do List Menu")
                .WithFields(new[] { new EmbedFieldBuilder().WithName($"To-Do Lists:").WithValue(toDoListDescription) });
            ComponentBuilder component = new ComponentBuilder()
                .WithSelectMenu(new SelectMenuBuilder()
                    .WithCustomId("settings-comp-listselect")
                    .WithOptions(toDoListEntries.Select(x => new SelectMenuOptionBuilder()
                        .WithValue(x).WithDefault(x == instance.toDoListName)).ToList()))
                .WithButton("Settings", "settings-comp-listsettingsbutton", disabled:instance.toDoListName == String.Empty);
            return new MessageContext(embed.Build(), component);
        }

        public async Task<MessageContext?> HandleInteraction(ToDoSettingsInstance instance, SocketInteraction interaction)
        {
            if (interaction.Type is InteractionType.MessageComponent)
            {
                IComponentInteraction componentInteraction = (IComponentInteraction)interaction;
                switch (componentInteraction.Data.CustomId)
                {
                    case "settings-comp-listselect":
                        {
                            return await HandleListSelect(instance, componentInteraction);
                        }
                    case "settings-comp-listsettingsbutton":
                        {
                            await HandleSettingsButton(instance, interaction);
                            break;
                        }
                }
            }
            return null;
        }

        public async Task<MessageContext> HandleListSelect(ToDoSettingsInstance instance, IComponentInteraction interaction)
        {
            instance.toDoListName = interaction.Data.Values.First();
            return await BuildPage(instance);
        }

        public async Task HandleSettingsButton(ToDoSettingsInstance instance, SocketInteraction interaction)
        {
            await _settingsService.OpenSettings(interaction, instance.toDoListName);
        }
    }
}
