﻿using Discord;
using Discord.Rest;
using InnoTasker.Data;
using InnoTasker.Data.Databases;
using InnoTasker.Data.ToDo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnoTasker.Services.Interfaces
{
    //This is a bridge between other services and the guild database
    public interface IGuildService
    {
        public Task<GuildData> GetGuildData(ulong guildID);
        public Task<GuildData?> GetGuildDataFromList(ToDoList list);

        public Task<ToDoList> GetToDoList(ulong guildID, string listName);
        public Task<ToDoList?> GetToDoListFromChannel(ulong channelID);
        public Task<ToDoList?> GetToDoListFromChannel(ulong guildID, ulong channelID);

        public Task SaveGuild(ulong guildID);

        public Task AddNewList(ulong guildID, ToDoList list);
        public Task RemoveList(ulong guildId, string listName);
        public Task RemoveList(ulong guildID, ToDoList list);

        public Task<List<IUserMessage>> GetListMessages();
        public Task<List<GuildData>> GetGuildDataList();
    }
}
