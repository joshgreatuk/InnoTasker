using Discord;
using InnoTasker.Data.ToDo;
using Newtonsoft.Json;
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

        [JsonIgnore]
        public bool IsChecked { get; set; } = false;

        public GuildData() {}
        public GuildData(ulong iD)
        {
            ID = iD;
        }

        public async Task<ToDoList> GetToDoList(string listName) => Lists.FirstOrDefault(x => x.Name == listName);
    }
}
