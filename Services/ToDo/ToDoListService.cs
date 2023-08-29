using InnoTasker.Services.Interfaces;
using InnoTasker.Services.Interfaces.ToDo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnoTasker.Services.ToDo
{
    public class ToDoListService : InnoServiceBase, IToDoListService
    {
        private readonly IGuildService _guildService;

        public ToDoListService(ILogger logger, IGuildService guildService) : base(logger)
        {
            _guildService = guildService;
        }

        public async Task InitService()
        {
            //Update the todo list and message
        }
    }
}
