using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnoTasker.Services.Interfaces.ToDo
{
    public interface IToDoUpdateService
    {
        public Task CreateToDoTask();
        public Task DeleteToDoTask();

        public Task ChangeTaskDescription();

        public Task CompleteTask();
        public Task UnCompleteTask();

        public Task TaskAddUser();
        public Task TaskRemoveUser();

        public Task TaskAddStage();
        public Task TaskRemoveStage();

        public Task TaskAddCategory();
        public Task TaskRemoveCategory();
    }
}
