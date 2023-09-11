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

        public Task UpdateToDoList(ulong guildId, string listName, Dictionary<string, string> renamedCategories = null, Dictionary<string, string> renamedStages = null);
        public Task UpdateToDoList(ToDoList list, Dictionary<string, string> renamedCategories=null, Dictionary<string,string> renamedStages=null);

        public Task UpdateToDoItem(ulong guildId, string listName, int itemID);
        public Task UpdateToDoItem(ToDoList list, int itemID);
        public Task UpdateToDoItem(ToDoItem item);

        public Task AddToDoList(ulong guildID, ToDoList list);
        public Task DeleteToDoList(ulong guildID, string toDoName);
    }
}
