using Discord;
using Discord.Rest;
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
        public ITextChannel ListChannel { get; set; }
        public ITextChannel CommandChannel { get; set; }
        public IForumChannel? ForumChannel { get; set; }

        public List<string> Categories { get; set; } = new();
        public List<string> Stages { get; set; } = new();

        public ITextChannel? MessageChannel { get; set; }
        public IUserMessage? Message { get; set; }
        public List<ToDoItem> Items { get; set; } = new();

        public Dictionary<ulong, ListUserPermissions> UserPermissions { get; set; } = new();
        public Dictionary<ulong, ListUserPermissions> RolePermissions { get; set; } = new();

        public async Task<ToDoItem> GetToDoItem(int id) => Items.FirstOrDefault(x => x.ID == id);
        public async Task<ToDoItem> GetToDoItem(string itemName) => Items.FirstOrDefault(x => x.Name == itemName);
    }
}
