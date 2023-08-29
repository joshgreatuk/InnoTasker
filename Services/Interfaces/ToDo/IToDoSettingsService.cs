using Discord;
using InnoTasker.Data;
using InnoTasker.Modules.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnoTasker.Services.Interfaces.ToDo
{
    public interface IToDoSettingsService
    { 
        public MessageContext GetToDoListPage();

        public MessageContext GetLastSettingsPage(ulong interactionID);
        public MessageContext GetSettingsPage(ulong interactionID, int index);
        public MessageContext GetNextSettingsPage(ulong interactionID);

        public void Shutdown();
    }
}
