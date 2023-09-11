using InnoTasker.Data.ToDo;
using InnoTasker.Services.Interfaces;
using InnoTasker.Services.Interfaces.ToDo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
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

        public async Task UpdateToDoList(ulong guildID, string listName, Dictionary<string, string> renamedCategories = null, Dictionary<string, string> renamedStages = null) => 
            await UpdateToDoList(await _guildService.GetToDoList(guildID, listName), renamedCategories, renamedStages);
        public async Task UpdateToDoList(ToDoList list, Dictionary<string, string> renamedCategories=null, Dictionary<string, string> renamedStages=null)
        {
            //Check and update channels

            //While going through, check for removed and renamed categories, check renamed before removed
        }

        public async Task UpdateToDoItem(ulong guildID, string listName, int itemID) => 
            await UpdateToDoItem(await _guildService.GetToDoList(guildID, listName), itemID);
        public async Task UpdateToDoItem(ToDoList list, int itemID) => 
            await list.GetToDoItem(itemID);
        public async Task UpdateToDoItem(ToDoItem item)
        {

        }

        public async Task DeleteToDoList(ulong guildID, string toDoName)
        {

        }
    }
}
