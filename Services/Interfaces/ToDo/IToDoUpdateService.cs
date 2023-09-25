using Discord;
using InnoTasker.Data.ToDo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnoTasker.Services.Interfaces.ToDo
{
    public interface IToDoUpdateService
    {
        public Task CreateToDoTask(ulong guildID, string listName, ToDoItem newItem);
        public Task DeleteToDoTask(ulong guildID, string listName, int taskID, ulong user);

        public Task ChangeTaskDescription(ulong guildID, string listName, int taskID, string newDescription, ulong user);

        public Task CompleteTask(ulong guildID, string listName, int taskID, ulong user);
        public Task UnCompleteTask(ulong guildID, string listName, int taskID, ulong user);

        public Task TaskAddUser(ulong guildID, string listName, int taskID, IGuildUser user);
        public Task TaskRemoveUser(ulong guildID, string listName, int taskID, IGuildUser user);

        public Task TaskAddStage(ulong guildID, string listName, int taskID, string stage);
        public Task TaskRemoveStage(ulong guildID, string listName, int taskID, string stage);

        public Task TaskAddCategory(ulong guildID, string listName, int taskID, string category);
        public Task TaskRemoveCategory(ulong guildID, string listName, int taskID, string category);

        public Task<string> GetListNameFromChannel(ulong guildID, ulong channelID);
    }
}
