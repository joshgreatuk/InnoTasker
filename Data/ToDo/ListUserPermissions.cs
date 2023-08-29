using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnoTasker.Data.ToDo
{
    [Flags]
    public enum ListUserPermissions
    {
        None = 0,
        User = 1,
        Editor = 2,
        Admin = 4
    }
}
