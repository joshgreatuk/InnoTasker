using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnoTasker.Data.Admin
{
    public class AdminServiceData
    {
        public Dictionary<AdminPostType, AdminPost> Posts { get; set; } = new()
        {
            { AdminPostType.ServerStats, new(AdminPostType.ServerStats) }
        };
    }
}
