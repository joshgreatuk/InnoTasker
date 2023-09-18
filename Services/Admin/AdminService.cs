using Discord;
using InnoTasker.Data.Admin;
using InnoTasker.Services.Interfaces;
using InnoTasker.Services.Interfaces.Admin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnoTasker.Services.Admin
{
    public class AdminService : InnoServiceBase, IAdminService
    {
        private readonly IServiceProvider _services;

        public AdminService(ILogger logger, IServiceProvider services) : base(logger) 
        {
            _services = services;
        }

        private Timer _updateTimer;

        public async Task Init()
        {
            _updateTimer = new Timer(async x => await UpdatePosts(), null, TimeSpan.FromMinutes(1), TimeSpan.FromMinutes(1));
            await _logger.LogAsync(LogSeverity.Info, this, $"Initialized!");
        }

        public async Task UpdatePosts()
        {

        }

        public async Task UpdatePost(AdminPostType type)
        {

        }

        public async Task SetPostChannel(AdminPostType type, IGuild guild, IChannel channel)
        {

        }

        public async Task Shutdown()
        {
            //Update stats to offline, leave embeds to show offline
        }
    }
}
