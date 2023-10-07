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
    public enum ToDoListChannelType { List, Command, Forum }

    public class ToDoList
    {
        public int CurrentID { get; set; } = 0;
        public string Name { get; set; }

        public ulong ListChannelID { get; set; }
        public ulong CommandChannelID { get; set; }
        public ulong? ForumChannelID { get; set; }
        [JsonIgnore] public ITextChannel ListChannel { get; set; }
        [JsonIgnore] public ITextChannel CommandChannel { get; set; }
        [JsonIgnore] public IForumChannel? ForumChannel { get; set; }

        public List<string> Categories { get; set; } = new();
        public List<string> Stages { get; set; } = new();

        public ulong? MessageChannelID { get; set; }
        [JsonIgnore] public ITextChannel? MessageChannel { get; set; }
        public ulong? MessageID;
        [JsonIgnore] public IUserMessage? Message { get; set; }
        public List<ToDoItem> Items { get; set; } = new();

        public Dictionary<ulong, ListUserPermissions> UserPermissions { get; set; } = new();
        public Dictionary<ulong, ListUserPermissions> RolePermissions { get; set; } = new();

        public async Task<ToDoItem> GetToDoItem(int id) => Items.FirstOrDefault(x => x.ID == id);
        public async Task<ToDoItem> GetToDoItem(string itemName) => Items.FirstOrDefault(x => x.Name == itemName);

        public async Task<ToDoItem?> GetToDoItemFromChannel(ulong channelID) => Items.FirstOrDefault(x => x.ForumPostID == channelID);
    }
}
