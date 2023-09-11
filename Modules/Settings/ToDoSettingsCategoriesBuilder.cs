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
    public class ToDoSettingsCategoriesBuilder : ISettingsPageBuilder
    {
        public async Task<MessageContext> BuildPage(ToDoSettingsInstance instance)
        {
            List<string> categoryEntries = new();
            categoryEntries.AddRange(instance.categoryList.Select(x => $"{x}"));
            List<string> stageEntries = new();
            stageEntries.AddRange(instance.stageList.Select(x => $"{x}"));

            EmbedBuilder embed = new EmbedBuilder()
                .WithTitle($"{instance.toDoListName} settings: Categories/Stages")
                .WithFields(new EmbedFieldBuilder[]
                {
                    new EmbedFieldBuilder().WithName("Categories").WithValue(categoryEntries.Count > 0 ? string.Join('\n', categoryEntries) : "None"),
                    new EmbedFieldBuilder().WithName("Stages").WithValue(stageEntries.Count > 0 ? string.Join('\n', stageEntries) : "None")
                })
                .WithFooter("Use '/admin (add/remove/rename)-(category/stage)' commands to modify the lists");

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
