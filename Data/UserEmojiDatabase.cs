using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnoTasker.Data
{
    public class UserEmojiDatabase : Database<ulong, UserEmoji>
    {
        public UserEmojiDatabase(IServiceProvider services, string path) : base(services, path) { }
    }
}
