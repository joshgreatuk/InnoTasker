using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnoTasker.Data.ToDo
{
    public class ToDoItem
    {
        public string Name { get; set; }
        public string Description { get; set; }

        public string Category { get; set; }
        public string Stage { get; set; }

        public List<ulong> AssignedUsers { get; set; }
        public ulong ForumPostID { get; set; }
        public string? CachedToDoEntry { get; set; }
        //Custom fields
    }
}
