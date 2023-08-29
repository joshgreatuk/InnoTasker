using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnoTasker.Services.Interfaces.ToDo
{
    public class ToDoSettingsInstance
    {
        public ulong interactionID;
        public int pageIndex = 0;

        public ToDoSettingsInstance(ulong interactionID)
        {
            this.interactionID = interactionID;
        }
    }
}
