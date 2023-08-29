using InnoTasker.Services.Interfaces;
using InnoTasker.Services.Interfaces.ToDo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnoTasker.Services.ToDo
{
    public class ToDoForumService : IToDoForumService
    {
        private readonly IGuildService _guildService;

        public ToDoForumService(IGuildService guildService)
        {
            _guildService = guildService;
        }

        public bool IsListForumEnabled(ulong guildID, string listName)
        {
            return _guildService.GetToDoList(guildID, listName).ForumChannelID != null;
        }
    }
}
