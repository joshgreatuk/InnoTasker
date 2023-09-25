using Discord;
using Discord.Rest;
using Discord.WebSocket;
using InnoTasker.Data;
using InnoTasker.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnoTasker.Data.ToDo
{
    public enum ToDoSettingsContext { New, Existing }
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
        public ToDoSettingsContext context;
        public int pageIndex = 0;

        //Channels Page
        public ITextChannel? toDoChannel;
        public ITextChannel? toDoCommandChannel;
        public IForumChannel? toDoForumChannel;

        //Categories Page
        public List<string> categoryList = new();
        public List<string> stageList = new();
        public Dictionary<string, string> categoriesRenamed = new();
        public Dictionary<string, string> stagesRenamed = new();

        //Permissions Page
        public Dictionary<ulong, ListUserPermissions> userPermissions = new();
        public Dictionary<ulong, ListUserPermissions> rolePermissions = new();

        public ToDoSettingsInstance(ulong interactionID)
        {
            this.interactionID = interactionID;
        }

        public async Task GrabGuildSettigns(IGuildService guildService)
        {
            ToDoList listData = await GetListData(guildService);

            toDoChannel = listData.ListChannel;
            toDoCommandChannel = listData.CommandChannel;
            toDoForumChannel = listData.ForumChannel;

            categoryList = listData.Categories;
            stageList = listData.Stages;

            userPermissions = listData.UserPermissions;
            rolePermissions = listData.RolePermissions;
        }

        public async Task SaveGuildSettings(IGuildService guildService)
        {
            ToDoList? listData = await GetListData(guildService);

            if (listData == null)
            {
                listData = new ToDoList();
                listData.Name = toDoListName;
                await guildService.AddNewList(guildID, listData);
            }
            listData.ListChannelID = toDoChannel.Id;
            listData.ListChannel = toDoChannel;
            listData.CommandChannelID = toDoCommandChannel.Id;
            listData.CommandChannel = toDoCommandChannel;
            listData.ForumChannelID = toDoForumChannel != null ? toDoForumChannel.Id : null;
            listData.ForumChannel = toDoForumChannel;

            listData.Categories = categoryList;
            listData.Stages = stageList;

            listData.UserPermissions = userPermissions;
            listData.RolePermissions = rolePermissions;
        }

        public async Task<ToDoList> GetListData(IGuildService guildService)
        {
            return await guildService.GetToDoList(guildID, toDoListName);
        }
    }
}
