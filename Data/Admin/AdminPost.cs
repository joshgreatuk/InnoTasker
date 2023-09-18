using Discord;
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
        public IGuild? Guild { get; set; }
        public IChannel? Channel { get; set; }
        public IMessage? Message { get; set; }

        public AdminPost(AdminPostType type)
        {
            Type = type;
        }
    }
}
