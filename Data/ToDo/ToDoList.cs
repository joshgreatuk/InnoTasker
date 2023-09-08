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
        public int CurrentID { get; set; }
        public string Name { get; set; }
        public ulong ListChannelID { get; set; }
        public ulong CommandChannelID { get; set; }
        public ulong? ForumChannelID { get; set; }

        public List<string> Categories { get; set; } = new();
        public List<string> Stages { get; set; } = new();

        public ulong MessageID { get; set; } = new();
        public List<ToDoItem> Items { get; set; } = new();

        public Dictionary<ulong, ListUserPermissions> UserPermissions { get; set; } = new();
        public Dictionary<ulong, ListUserPermissions> RolePermissions { get; set; } = new();

        public async Task<ToDoItem> GetToDoItem(int id) => Items.FirstOrDefault(x => x.ID == id);
        public async Task<ToDoItem> GetToDoItem(string itemName) => Items.FirstOrDefault(x => x.Name == itemName);
    }
}
