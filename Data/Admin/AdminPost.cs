using Discord;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnoTasker.Data.Admin
{
    public class AdminPost
    {
        public AdminPostType Type { get; set; }
        public ulong? GuildID { get; set; }
        [JsonIgnore] public IGuild? Guild { get; set; }
        public ulong? ChannelID { get; set; }
        [JsonIgnore] public ITextChannel? Channel { get; set; }
        public ulong? MessageID { get; set; }
        [JsonIgnore] public IUserMessage? Message { get; set; }

        public AdminPost(AdminPostType type)
        {
            Type = type;
        }
    }
}
