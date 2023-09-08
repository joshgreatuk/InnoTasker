using Discord.Interactions;
using Discord.Rest;
using InnoTasker.Data;
using InnoTasker.Modules.Autocomplete;
using InnoTasker.Services.ToDo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnoTasker.Modules
{
    [Group("admin", "Server admin commands")]
    [RequireUserPermission(Discord.GuildPermission.Administrator)]
    public class AdminModule : InteractionModuleBase<SocketInteractionContext>
    {
        private readonly ToDoSettingsService _toDoSettingsService;

        public AdminModule(ToDoSettingsService toDoSettingsService)
        {
            _toDoSettingsService = toDoSettingsService;
        }

        public async Task OpenToDoListMenu()
        {
            await DeferAsync(true);
            if (await _toDoSettingsService.OpenToDoListPage(Context.Interaction))
            {
                await RespondAsync();
            }
            else
            {
                await RespondAsync(embed: new Discord.EmbedBuilder().WithTitle($"Error")
                    .WithDescription("Sorry, there was an error opening the to do list menu").Build());
            }
        }

        public async Task OpenToDoSettingsMenu([Autocomplete(typeof(ToDoListAutocomplete))]string toDoName)
        {
            await DeferAsync(true);
            if (await _toDoSettingsService.OpenSettings(Context.Interaction, toDoName))
            {
                await RespondAsync(ephemeral:true);
            }
            else
            {
                await RespondAsync(ephemeral: true, embed: new Discord.EmbedBuilder().WithTitle($"Error")
                    .WithDescription("Sorry, there was an error opening the settings menu").Build());
            }
        }
    }
}
