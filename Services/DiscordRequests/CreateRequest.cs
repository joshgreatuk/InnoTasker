using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnoTasker.Services.DiscordRequests
{
    public class CreateRequest : DiscordRequest
    {
        public Optional<Embed> embeds = new();
        public ComponentBuilder components = new();
        public bool ephemeral = false;

        public CreateRequest(ulong guildID, ulong channelID) : base(guildID, channelID)
        {

        }

        public CreateRequest(IGuild guild, IMessageChannel channel) : base(guild, channel)
        {

        }
    }
}
