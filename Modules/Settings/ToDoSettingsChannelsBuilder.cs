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
            List<string> entries = new();
            entries.Add($"**To-Do list channel:** {(instance.toDoChannel != null ? MentionUtils.MentionChannel((ulong)instance.toDoChannel) : "None (Required)")}");
            entries.Add($"**To-Do commands channel:** {(instance.toDoCommandChannel != null ? MentionUtils.MentionChannel((ulong)instance.toDoCommandChannel) : "None (Required)")}");
            entries.Add($"**To-Do task forum channel:** {(instance.toDoForumChannel != null ? MentionUtils.MentionChannel((ulong)instance.toDoForumChannel) : "None (Optional)")}");

            EmbedBuilder embed = new EmbedBuilder()
                .WithTitle($"{instance.toDoListName} settings: Channels")
                .WithDescription(string.Join('\n', entries))
                .WithFooter("Use '/admin set-todo-channel' to update channels or '/admin unset-todo-channel' to unset a channel");

            ComponentBuilder component = new();
            return new(embed.Build(), component);
        }

        public async Task<MessageContext?> HandleInteraction(ToDoSettingsInstance instance, SocketInteraction interaction)
        {
            throw new NotImplementedException(); //No extra interaction to handle
        }

        public async Task<bool> CanProceed(ToDoSettingsInstance instance)
        {
            return instance.toDoChannel != null && instance.toDoCommandChannel != null;
        }
    }
}
