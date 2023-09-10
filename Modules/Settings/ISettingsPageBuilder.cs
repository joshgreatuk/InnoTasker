using Discord.WebSocket;
using InnoTasker.Data;
using InnoTasker.Services.Interfaces.ToDo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnoTasker.Modules.Settings
{
    public interface ISettingsPageBuilder
    {
        public Task<MessageContext> BuildPage(ToDoSettingsInstance instance);

        public Task<MessageContext?> HandleInteraction(ToDoSettingsInstance instance, SocketInteraction interaction);

        public Task<bool> CanProceed(ToDoSettingsInstance instance); //TO-DO: Ensure this is built into settings page builders
    }
}
