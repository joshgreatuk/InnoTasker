using Discord;
using Discord.Commands;
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
    public class ToDoSettingsChannelsBuilder : ISettingsPageBuilder
    {
        public async Task<MessageContext> BuildPage(ToDoSettingsInstance instance)
        {
            EmbedBuilder embed = new EmbedBuilder()
                .WithTitle($"{instance.toDoListName} settings: Channels")
                .WithFooter("Use /");

            ComponentBuilder component = new();
            return new(embed.Build(), component);
        }

        public async Task<MessageContext?> HandleInteraction(ToDoSettingsInstance instance, SocketInteraction interaction)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> CanProceed(ToDoSettingsInstance instance)
        {
            return instance.toDoChannel != null && instance.toDoCommandChannel != null;
        }
    }
}
