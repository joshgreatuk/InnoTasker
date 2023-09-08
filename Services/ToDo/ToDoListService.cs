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

        public async Task UpdateToDoList(ulong guildID, string listName) => 
            await UpdateToDoList(await _guildService.GetToDoList(guildID, listName));
        public async Task UpdateToDoList(ToDoList list)
        {
            //Check and update channels
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

        //TO-DO: write Category/Stage updates
        #region Category/Stage Updates 
        public async Task CategoryRemoved(ToDoList list, string category)
        {
            foreach (ToDoItem item in list.Items.Where(x => x.Categories.Contains(category)))
            {
                item.Categories.Remove(category);
                await UpdateToDoItem(item);
            }
        }

        public async Task CategoryRenamed(ToDoList list, string category, string newCategory)
        {
            foreach (ToDoItem item in list.Items.Where(x => x.Categories.Contains(category)))
            {
                item.Categories[item.Categories.IndexOf(category)] = newCategory;
                await UpdateToDoItem(item);
            }
        }

        public async Task StageRemoved(ToDoList list, string stage)
        {
            foreach (ToDoItem item in list.Items.Where(x => x.Stages.Contains(stage)))
            {
                item.Stages.Remove(stage);
                await UpdateToDoItem(item);
            }
        }

        public async Task StageRenamed(ToDoList list, string stage, string newStage)
        {
            foreach (ToDoItem item in list.Items.Where(x => x.Stages.Contains(stage)))
            {
                item.Stages[item.Stages.IndexOf(stage)] = newStage;
                await UpdateToDoItem(item);
            }
        }
        #endregion
    }
}
