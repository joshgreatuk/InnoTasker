using Discord;
using Discord.Rest;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnoTasker.Data.ToDo
{
    public class ToDoItem
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public List<string> Categories { get; set; } = new();
        public List<string> Stages { get; set; } = new();

        public bool IsComplete { get; set; } = false;

        public List<ulong> AssignedUsers { get; set; } = new();


        public ulong? ForumPostID { get; set; }
        [JsonIgnore] public IThreadChannel? ForumPost { get; set; }
        public ulong? StatusMessageID { get; set; }
        [JsonIgnore] public IUserMessage? StatusMessage { get; set; }

        public ulong? SorryMessageID { get; set; }
        [JsonIgnore] public IUserMessage? SorryMessage { get; set; }

        public string? CachedToDoEntry { get; set; }

        public Queue<ItemUpdate> ItemUpdateQueue { get; set; } = new();
        //Custom fields
    }
}
