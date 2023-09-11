using Discord;
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
    public class ToDoSettingsPermissionsBuilder : ISettingsPageBuilder
    {
        public async Task<MessageContext> BuildPage(ToDoSettingsInstance instance)
        {
            List<string> roleEntries = new();
            roleEntries.AddRange(instance.rolePermissions.OrderByDescending(x => x.Value).Select(x => $"{MentionUtils.MentionRole(x.Key)} : {x.Value}"));
            List<string> userEntries = new();
            userEntries.AddRange(instance.userPermissions.OrderByDescending(x => x.Value).Select(x => $"{MentionUtils.MentionUser(x.Key)} : {x.Value}"));

            EmbedBuilder embed = new EmbedBuilder()
                .WithTitle($"{instance.toDoListName} settings: Permissions")
                .WithFields(new EmbedFieldBuilder[]
                {
                    new EmbedFieldBuilder().WithName("Role Permissions").WithValue(roleEntries.Count > 0 ? string.Join('\n', roleEntries) : "None"),
                    new EmbedFieldBuilder().WithName("User Permissions").WithValue(userEntries.Count > 0 ? string.Join('\n', userEntries) : "None")
                })
                .WithFooter($"Use '/admin set-(role/user)-permission' to add/remove/modify permissions");

            ComponentBuilder component = new();
            return new(embed.Build(), component);
        }

        public async Task<MessageContext?> HandleInteraction(ToDoSettingsInstance instance, SocketInteraction interaction)
        {
            throw new NotImplementedException(); //No extra interactions to handle
        }

        public async Task<bool> CanProceed(ToDoSettingsInstance instance)
        {
            return true;
        }
    }
}
