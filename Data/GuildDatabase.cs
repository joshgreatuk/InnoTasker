using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnoTasker.Data
{
    public class GuildDatabase : Database<ulong, GuildData>
    {
        public GuildDatabase(IServiceProvider services, string path) : base(services, path) { }
    }
}
