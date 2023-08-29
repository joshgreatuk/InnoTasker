using InnoTasker.Services.Interfaces;
using InnoTasker.Services.Interfaces.ToDo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnoTasker.Services.ToDo
{
    public class ToDoUpdateService : InnoServiceBase, IToDoUpdateService
    {
        private readonly IGuildService _guildService;
        private readonly IToDoListService _toDoListService;
        private readonly IToDoForumService _toDoForumService;

        public ToDoUpdateService(ILogger logger, IGuildService guildService, IToDoListService toDoListService, IToDoForumService toDoForumService) : base(logger)
        {
            _guildService = guildService;
            _toDoListService = toDoListService;
            _toDoForumService = toDoForumService;
        }
    }
}
