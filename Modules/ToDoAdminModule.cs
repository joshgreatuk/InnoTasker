using Discord;
using Discord.Interactions;
using InnoTasker.Modules.Autocomplete;
using InnoTasker.Services.Interfaces.ToDo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace InnoTasker.Modules
{
    [Group("to-do-admin", "Admin commands for a to-do list")]
    public class ToDoAdminModule : InteractionModuleBase<SocketInteractionContext>
    {
        private readonly IToDoUpdateService _updateService;

        public ToDoAdminModule(IToDoUpdateService updateService)
        {
            _updateService = updateService;
        }

    }
}
