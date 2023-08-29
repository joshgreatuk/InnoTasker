using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnoTasker.Services.Interfaces.ToDo
{
    public interface IToDoForumService
    {
        public bool IsListForumEnabled(ulong guildID, string listName);

        public Task InitService();
    }
}
