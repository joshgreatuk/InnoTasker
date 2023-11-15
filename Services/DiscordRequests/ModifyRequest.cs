using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnoTasker.Services.DiscordRequests
{
    public class ModifyRequest : DiscordRequest
    {
        public ulong targetMessageID;
        public IUserMessage? targetMessage;

        public Action<MessageProperties> func;

        public ModifyRequest(ulong guildID, ulong channelID, ulong targetMessageID, Action<MessageProperties> func) : base(guildID, channelID)
        {
            this.targetMessageID = targetMessageID;
            this.func = func;
        }

        public ModifyRequest(IGuild guild, IUserMessage message, Action<MessageProperties> func) : base(guild, message)
        {
            this.targetMessageID = message.Id;
            this.targetMessage = message;
            this.func = func;
        }
    }
}
