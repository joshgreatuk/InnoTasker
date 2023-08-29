using InnoTasker.Services.Interfaces;
using InnoTasker.Services.Interfaces.ToDo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnoTasker.Services.ToDo
{
    public class ToDoSettingsService : InnoServiceBase, IToDoSettingsService
    {
        private readonly IGuildService _guildService;
        private readonly IToDoListService _toDoListService;

        public ToDoSettingsService(ILogger logger, IGuildService guildService, IToDoListService toDoListService) : base(logger)
        {
            _guildService = guildService;
            _toDoListService = toDoListService;
        }
    }
}
