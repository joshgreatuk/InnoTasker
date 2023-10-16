using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnoTasker.Data.ToDo
{
    //A data class for the ToDoListService holding sorted items and embeds
    public class ToDoListField
    {
        public string fieldName;
        public List<ToDoItem> sortedItems = new();

        public ToDoListField() { }
        public ToDoListField(List<ToDoItem> sortedItems)
        {
            this.sortedItems = sortedItems;
        }
    }
}
