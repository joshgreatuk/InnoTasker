using InnoTasker.Data;
using InnoTasker.Data.ToDo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnoTasker.Services.Interfaces
{
    //This is a bridge between other services and the guild database
    public interface IGuildService
    {
        public GuildData GetGuildData(ulong guildID);

        public ToDoList GetToDoList(ulong guildID, string listName);
    }
}
