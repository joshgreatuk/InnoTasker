using Discord;
using InnoTasker.Data.ToDo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnoTasker.Data
{
    public class GuildData
    {
        public ulong ID { get; set; }
        public List<ToDoList> Lists { get; set; } = new();

        public GuildData(ulong iD)
        {
            ID = ID;
        }

        public ToDoList GetToDoList(string listName) => Lists.FirstOrDefault(x => x.Name == listName);
    }
}
