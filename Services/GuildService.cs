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
    public class GuildService : InnoServiceBase, IGuildService
    {
        private readonly GuildDatabase _guildDatabase;
        private readonly DiscordSocketClient _client;

        public GuildService(ILogger logger, GuildDatabase data, DiscordSocketClient client) : base(logger)
        {
            _guildDatabase = data;
            _client = client;

            client.JoinedGuild += OnGuildJoin;
            client.LeftGuild += OnGuildLeave;

            client.Ready += OnClientReady;

            _logger.Log(LogSeverity.Info, this, $"GuildService initialized");
        }

        public async Task OnClientReady()
        {
            //Check through to see if theres any guilds we dont have in the database
            int guildsAdded = 0;
            int guildsRemoved = 0;
            foreach (SocketGuild guild in _client.Guilds)
            {
                if (_guildDatabase.TryGetValue(guild.Id, out GuildData data))
                {
                    data.IsChecked = true;
                }
                else
                {
                    await OnGuildJoin(guild);
                    guildsAdded++;
                }
            }
            List<GuildData> toRemove = _guildDatabase.Values.Where(x => !x.IsChecked).ToList();
            foreach (GuildData data in toRemove)
            {
                await OnGuildLeave(data.ID);
                guildsRemoved++;
            }
            await _logger.LogAsync(LogSeverity.Info, this, $"Guild database check completed, added {guildsAdded} missing guilds, removed {guildsRemoved} ");
        }

        public async Task OnGuildJoin(SocketGuild guild)
        {
            GuildData newData = new GuildData(guild.Id);
            newData.IsChecked = true;
            _guildDatabase.Add(guild.Id, newData);
            await _logger.LogAsync(LogSeverity.Debug, this, $"Joined guild '{guild.Id}'");
        }

        public async Task OnGuildLeave(SocketGuild guild)
        {
            await OnGuildLeave(guild.Id);
        }
        public async Task OnGuildLeave(ulong guildID)
        {
            _guildDatabase.Remove(guildID);
            await _logger.LogAsync(LogSeverity.Debug, this, $"Left guild '{guildID}'");
        }

        public GuildData GetGuildData(ulong guildID)
        {
            return _guildDatabase[guildID];
        }

        public ToDoList GetToDoList(ulong guildID, string listName)
        {
            return GetGuildData(guildID).GetToDoList(listName);
        }

        public ToDoList? GetToDoListFromChannel(ulong guildID, ulong channelID)
        {
            return GetGuildData(guildID).Lists.FirstOrDefault(x => x.ListChannelID == channelID 
                || x.CommandChannelID == channelID 
                || x.ForumChannelID == channelID);
        }
    }
}
