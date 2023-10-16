using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnoTasker.Data.ToDo
{
    public class ListSortBy<ListSortType>
    {
        public ListSortType SortType { get; set; }
        public ListSortDirection Direction { get; set; }

        public ListSortBy() { }
        public ListSortBy(ListSortType sortType, ListSortDirection direction=ListSortDirection.Ascending)
        {
            SortType = sortType;
            Direction = direction;
        }

        public Func<ToDoItem, string> GetSubSortKey()
        {
            if (SortType is not SubListSortType subSortType) return null;
            switch (subSortType)
            {
                case SubListSortType.ID: return x => x.ID.ToString();
                case SubListSortType.Name: return x => x.Name;
                case SubListSortType.Stage: return x => x.Stages.First();
            }
            return null;
        }
    }
}
