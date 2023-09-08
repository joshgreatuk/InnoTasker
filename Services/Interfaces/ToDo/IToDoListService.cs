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

        public Task UpdateToDoList(ulong guildId, string listName);
        public Task UpdateToDoList(ToDoList list);

        public Task UpdateToDoItem(ulong guildId, string listName, int itemID);
        public Task UpdateToDoItem(ToDoList list, int itemID);
        public Task UpdateToDoItem(ToDoItem item);

        public Task DeleteToDoList(ulong guildID, string toDoName);

        public Task CategoryRemoved(ToDoList list, string category);
        public Task CategoryRenamed(ToDoList list, string oldName, string newName);

        public Task StageRemoved(ToDoList list, string stage);
        public Task StageRenamed(ToDoList list, string oldName, string newName);
    }
}
