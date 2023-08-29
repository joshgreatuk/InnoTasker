using InnoTasker.Services.Interfaces;
using InnoTasker.Services.Interfaces.ToDo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnoTasker.Services.ToDo
{
    public class ToDoListService : IToDoListService
    {
        private readonly IGuildService _guildService;

        public ToDoListService(IGuildService guildService)
        {
            _guildService = guildService;
        }
    }
}
