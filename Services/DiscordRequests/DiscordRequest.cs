using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnoTasker.Services.DiscordRequests
{
    public class DiscordRequest
    {
        public ulong guildTargetID;
        public IGuild? guild;
        public ulong channelTargetID;
        public IMessageChannel? channel;

        public DiscordRequest(ulong guildTargetID, ulong channelTargetID)
        {
            this.guildTargetID = guildTargetID;
            this.channelTargetID = channelTargetID;
        }

        public DiscordRequest(IGuild guild, IMessageChannel channel)
        {
            this.guildTargetID = guild.Id;
            this.guild = guild;
            this.channelTargetID = channel.Id;
            this.channel = channel;
        }

        public DiscordRequest(IGuild guild, IUserMessage message)
        {
            this.guildTargetID = guild.Id;
            this.guild = guild;
            this.channelTargetID = message.Channel.Id;
            this.channel = message.Channel;
        }
    }
}
