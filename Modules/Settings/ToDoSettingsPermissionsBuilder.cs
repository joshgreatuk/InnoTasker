﻿using Discord.WebSocket;
using InnoTasker.Data;
using InnoTasker.Services.Interfaces;
using InnoTasker.Services.Interfaces.ToDo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnoTasker.Modules.Settings
{ 
    public class ToDoSettingsPermissionsBuilder : ISettingsPageBuilder
    {
        public async Task<MessageContext> BuildPage(ToDoSettingsInstance instance)
        {
            throw new NotImplementedException();
        }

        public async Task<MessageContext?> HandleInteraction(ToDoSettingsInstance instance, SocketInteraction interaction)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> CanProceed(ToDoSettingsInstance instance)
        {
            return true;
        }
    }
}
