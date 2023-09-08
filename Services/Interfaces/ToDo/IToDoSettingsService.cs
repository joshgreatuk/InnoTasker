using Discord;
using Discord.WebSocket;
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
        public Task<bool> OpenToDoListPage(SocketInteraction interaction);
        public Task<bool> OpenSettings(SocketInteraction interaction, string toDoListName); 

        public Task<MessageContext> GetLastSettingsPage(ulong interactionID);
        public Task<MessageContext> GetSettingsPage(ulong interactionID, int index);
        public Task<MessageContext> GetNextSettingsPage(ulong interactionID);

        public Task HandleInteraction(SocketInteraction interaction);

        public Task CloseInstance(ulong interactionID);

        public Task<bool> UpdateInstance(ulong interactionID, MessageContext context);

        public void Shutdown();
    }
}
