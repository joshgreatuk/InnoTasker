using Discord.WebSocket;
using InnoTasker.Data.ToDo;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnoTasker.Data.Databases
{
    public class GuildDatabase : Database<ulong, GuildData>
    {
        private readonly DiscordSocketClient _client;

        public GuildDatabase(IServiceProvider services, string path) : base(services, path) 
        {
            _client = services.GetRequiredService<DiscordSocketClient>();
        }

        public override void InitEntry(ref GuildData value)
        {
            SocketGuild targetGuild = _client.GetGuild(value.ID);
            foreach (ToDoList list in value.Lists)
            {
                list.ListChannel = targetGuild.GetTextChannel(list.ListChannelID);
                list.CommandChannel = targetGuild.GetTextChannel(list.CommandChannelID);
                list.ForumChannel = list.ForumChannelID != null ? targetGuild.GetForumChannel((ulong)list.ForumChannelID) : null;

                list.MessageChannel = list.MessageChannelID != null ? targetGuild.GetTextChannel((ulong)list.MessageChannelID) : null;
                list.Message = list.MessageID != null && list.MessageChannel != null ? list.MessageChannel.ModifyMessageAsync((ulong)list.MessageID, x => x.Content = "").Result : null;
            }
        }
    }
}
