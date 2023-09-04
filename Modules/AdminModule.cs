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
    public class AdminModule : InteractionModuleBase<SocketInteractionContext>
    {
        private readonly ToDoSettingsService _toDoSettingsService;

        public AdminModule(ToDoSettingsService toDoSettingsService)
        {
            _toDoSettingsService = toDoSettingsService;
        }

        public void OpenToDoListMenu()
        {
            
            MessageContext message = _toDoSettingsService.GetToDoListPage();
        }

        public void OpenToDoSettingsMenu([Autocomplete(typeof(ToDoListAutocomplete))]string toDoName)
        {

        }
    }
}
