using Discord.Rest;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnoTasker.Services.Interfaces.ToDo
{
    public enum ToDoSettingsInstanceMode { ToDoMenu, ToDoSettings }

    public class ToDoSettingsInstance
    {
        public ulong guildID;
        public ulong interactionID; //This is the channel ID
        public RestUserMessage message;
        public ToDoSettingsInstanceMode mode;

        //Shared
        public string toDoListName;

        //Settings Mode
        public int pageIndex = 0;

        public ToDoSettingsInstance(ulong interactionID)
        {
            this.interactionID = interactionID;
        }
    }
}
