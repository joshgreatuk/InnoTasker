using InnoTasker.Services.Interfaces;
using InnoTasker.Services.Interfaces.ToDo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnoTasker.Services.ToDo
{
    public class ToDoForumService : InnoServiceBase, IToDoForumService
    {
        private readonly IGuildService _guildService;

        public ToDoForumService(ILogger logger, IGuildService guildService) : base(logger)
        {
            _guildService = guildService;
        }

        public bool IsListForumEnabled(ulong guildID, string listName)
        {
            return _guildService.GetToDoList(guildID, listName).Result.ForumChannelID != null;
        }

        public async Task InitService()
        {
            //Update forum messages
        }
    }
}
