using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnoTasker.Data.ToDo
{
    public enum MainListSortType
    {
        None,
        Category,
        Stage
    }
    
    public enum SubListSortType
    {
        ID,
        Name,
        Stage
    }

    public enum ListSortDirection
    {
        Ascending,
        Descending
    }
}
