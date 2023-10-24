using InnoTasker.Data.ToDo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnoTasker.Services.Interfaces.ToDo
{
    public interface IToDoListService
    {
        public Task InitService();

        public Task UpdateToDoList(ulong guildID, string listName, Dictionary<string, string> renamedCategories = null, Dictionary<string, string> renamedStages = null);
        public Task UpdateToDoList(ulong guildID, ToDoList list, Dictionary<string, string> renamedCategories=null, Dictionary<string,string> renamedStages=null);

        public Task UpdateToDoListMessage(ulong guildID, string listName);
        public Task UpdateToDoListMessage(ulong guildID, ToDoList list);

        public Task UpdateToDoItem(ulong guildID, string listName, int itemID);
        public Task UpdateToDoItem(ulong guildID, ToDoList list, int itemID);
        public Task UpdateToDoItem(ulong guildID, ToDoList list, ToDoItem item, bool updateMessage=true);

        public Task AddToDoList(ulong guildID, ToDoList list);
        public Task DeleteToDoList(ulong guildID, string toDoName);

        public Task AddToDoItem(ulong guildID, string listName, ToDoItem item);
        public Task AddToDoItem(ulong guildID, ToDoList list, ToDoItem item);

        public Task RemoveToDoItem(ulong guildID, string listName, int itemID);
        public Task RemoveToDoItem(ulong guildID, string listName, ToDoItem item);
        public Task RemoveToDoItem(ulong guildID, ToDoList list, int itemID);
        public Task RemoveToDoItem(ulong guildID, ToDoList list, ToDoItem item);

        public Task Shutdown();
    }
}
