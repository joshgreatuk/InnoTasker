using Discord;
using Discord.WebSocket;
using InnoTasker.Data;
using InnoTasker.Data.ToDo;
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
        public Task<bool> OpenSettings(SocketInteraction interaction, string toDoListName, ToDoSettingsContext context); 

        public Task<MessageContext> GetLastSettingsPage(ulong interactionID);
        public Task<MessageContext> GetSettingsPage(ulong interactionID, int index);
        public Task<MessageContext> GetCurrentSettingsPage(ulong interactionID);
        public Task<MessageContext> GetNextSettingsPage(ulong interactionID);

        public Task HandleInteraction(SocketInteraction interaction);

        public Task CloseInstance(ulong interactionID, string? message=null);

        public Task<bool> UpdateInstance(SocketInteraction interactionID, MessageContext context);

        public Task<bool> InstanceExists(ulong interactionID);

        public Task<ToDoSettingsInstance> GetSettingsInstance(ulong instanceID);
        public Task<string> GetCurrentInstanceListName(ulong interactionID);

        public Task<bool> SaveInstance(ulong instanceID);

        public Task Shutdown();
    }
}
