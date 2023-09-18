using Discord.Interactions;
using InnoTasker.Services.Interfaces.ToDo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnoTasker.Modules
{
    [Group("to-do-item-admin", "Admin commands for a task inside it's forum post")]
    public class ToDoItemAdminModule : InteractionModuleBase<SocketInteractionContext>
    {
        private readonly IToDoUpdateService _updateService;

        public ToDoItemAdminModule(IToDoUpdateService updateService)
        {
            _updateService = updateService;
        }
    }
}
