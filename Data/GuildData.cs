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
        public List<ToDoList> Lists { get; set; } = new();
    }
}
