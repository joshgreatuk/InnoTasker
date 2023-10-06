using Discord;
using InnoTasker.Data.ToDo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnoTasker.Services.Interfaces.ToDo
{
    public interface IToDoForumService
    {
        public Task InitService();

        public Task<bool> IsListForumEnabled(ulong guildID, string listName);
        public Task<bool> IsListForumEnabled(ToDoList list);

        public Task<bool> DoesTaskPostExist(ToDoItem item);

        public Task CreateTaskPost(ToDoList list, ToDoItem item);
        public Task CompleteTaskPost(ToDoItem item);
        public Task UnCompleteTaskPost(ToDoItem item);

        public Task AddUserTaskPost(ToDoItem item, IGuildUser user);
        public Task RemoveUserTaskPost(ToDoItem item, IGuildUser user);

        public Task UpdateStatusMessage(ToDoItem item);
        public Task<IUserMessage> ProcessUpdateMessages(ToDoItem item, Color? colour=null);
        
        public Task Shutdown();
    }
}
