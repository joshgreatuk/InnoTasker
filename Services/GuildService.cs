using Discord;
using Discord.WebSocket;
using InnoTasker.Data;
using InnoTasker.Data.Databases;
using InnoTasker.Data.ToDo;
using InnoTasker.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnoTasker.Services
{
    public class GuildService : IGuildService
    {
        private readonly ILogger _logger;
        private readonly GuildDatabase _guildDatabase;
        

        public GuildService(ILogger logger, GuildDatabase data, DiscordSocketClient client)
        {
            _logger = logger;
            _guildDatabase = data;

            client.JoinedGuild += OnGuildJoin;
            client.LeftGuild += OnGuildLeave;
            _logger.Log(LogSeverity.Info, this, $"GuildService initialized");
        }

        public async Task OnGuildJoin(SocketGuild guild)
        {
            GuildData newData = new GuildData(guild.Id);
            _guildDatabase.Add(guild.Id, newData);
            await _logger.LogAsync(LogSeverity.Debug, this, $"Joined guild '{guild.Id}'");
        }

        public async Task OnGuildLeave(SocketGuild guild)
        {
            _guildDatabase.Remove(guild.Id);
            await _logger.LogAsync(LogSeverity.Debug, this, $"Left guild '{guild.Id}'");
        }

        public GuildData GetGuildData(ulong guildID)
        {
            return _guildDatabase[guildID];
        }

        public ToDoList GetToDoList(ulong guildID, string listName)
        {
            return GetGuildData(guildID).GetToDoList(listName);
        }
    }
}
