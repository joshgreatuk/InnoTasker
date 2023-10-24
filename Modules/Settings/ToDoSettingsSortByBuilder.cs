using Discord;
using Discord.WebSocket;
using InnoTasker.Data;
using InnoTasker.Data.ToDo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnoTasker.Modules.Settings
{
    public class ToDoSettingsSortByBuilder : ISettingsPageBuilder
    {
        public async Task<MessageContext> BuildPage(ToDoSettingsInstance instance)
        {
            List<string> descriptionStrings = new()
            {
                $"Main Sort Type: {instance.mainSortBy.SortType} ({instance.mainSortBy.Direction})",
                $"Sub Sort Type: {instance.subSortBy.SortType} ({instance.subSortBy.Direction})"
            };

            EmbedBuilder embed = new EmbedBuilder()
                .WithTitle($"{instance.toDoListName} settings: Sorting")
                .WithDescription(string.Join("\n", descriptionStrings))
                .WithFooter("Use /admin sortby (SetMain/SetSub/ToggleDirection) to set sorting modes!");

            ComponentBuilder components = new ComponentBuilder();

            return new(embed.Build(), components);
        }

        public async Task<MessageContext?> HandleInteraction(ToDoSettingsInstance instance, SocketInteraction interaction)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> CanProceed(ToDoSettingsInstance instance) => true;
    }
}
