using Discord.Interactions;
using InnoTasker.Services.Interfaces.ToDo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnoTasker.Modules
{
    [Group("to-do-item", "Commands relating a task in it's forum post")]
    public class ToDoItemModule : InteractionModuleBase<SocketInteractionContext>
    {
        private readonly IToDoUpdateService _updateService;

        public ToDoItemModule(IToDoUpdateService updateService)
        {
            _updateService = updateService;
        }
    }
}
