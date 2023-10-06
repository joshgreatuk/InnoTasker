using Discord.WebSocket;
using InnoTasker.Data.Admin;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnoTasker.Data.Databases
{
    public class AdminPostDatabase : Database<int, AdminPost>
    {
        private DiscordSocketClient _client;

        public AdminPostDatabase(IServiceProvider services, string path) : base(services, path) 
        { 
            _client = services.GetRequiredService<DiscordSocketClient>();
        }

        public override void InitEntry(ref AdminPost value)
        {
            value.Guild = value.GuildID != null ? _client.GetGuild((ulong)value.GuildID) : null;
            value.Channel = value.ChannelID != null && value.Guild != null ? value.Guild.GetTextChannelAsync((ulong)value.ChannelID).Result : null;
            value.Message = value.MessageID != null && value.Channel != null ? value.Channel.ModifyMessageAsync((ulong)value.MessageID, x => x.Content = "").Result : null;
        }
    }
}
