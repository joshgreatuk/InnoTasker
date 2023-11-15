using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnoTasker.Services.DiscordRequests
{
    public class DeleteRequest : DiscordRequest
    {
        public ulong targetMessageID;
        public IUserMessage? targetMessage;

        public DeleteRequest(ulong guildID, ulong channelID, ulong messageID) : base(guildID, channelID)
        {
            this.targetMessageID = messageID;
        }

        public DeleteRequest(IGuild guild, IUserMessage message) : base(guild, message)
        {
            this.targetMessageID = message.Id;
            this.targetMessage = message;
        }
    }
}
