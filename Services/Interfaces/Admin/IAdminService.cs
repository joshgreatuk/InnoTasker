using Discord;
using InnoTasker.Data.Admin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnoTasker.Services.Interfaces.Admin
{
    public interface IAdminService
    {
        public Task Init();

        public Task UpdatePosts();
        public Task UpdatePost(AdminPostType type);

        public Task SetPostChannel(AdminPostType type, IGuild guild, ITextChannel channel);

        public Task RefreshList(ulong guildID, string listName);

        public Task Shutdown();
    }
}
