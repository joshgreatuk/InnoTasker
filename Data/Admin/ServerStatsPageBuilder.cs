using Discord.WebSocket;
using InnoTasker.Data.ToDo;
using InnoTasker.Modules.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnoTasker.Data.Admin
{
    public class ServerStatsPageBuilder : ISettingsPageBuilder
    {
        public async Task<MessageContext> BuildPage(ToDoSettingsInstance settings)
        {
            throw new NotImplementedException();
        }

        public async Task<MessageContext?> HandleInteraction(ToDoSettingsInstance settings, SocketInteraction interaction)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> CanProceed(ToDoSettingsInstance settings) => true;
    }
}
