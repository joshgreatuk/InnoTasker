using Discord;
using InnoTasker.Data;
using InnoTasker.Data.Admin;
using InnoTasker.Data.Databases;
using InnoTasker.Modules.Settings;
using InnoTasker.Services.Interfaces;
using InnoTasker.Services.Interfaces.Admin;
using InnoTasker.Services.Interfaces.ToDo;
using Microsoft.Extensions.DependencyInjection;
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
        private readonly IToDoListService _toDoListService;
        private readonly AdminPostDatabase _adminPosts;

        private readonly Dictionary<AdminPostType, ISettingsPageBuilder> _adminPageBuilders;

        public AdminService(ILogger logger, IServiceProvider services) : base(logger) 
        {
            _services = services;
            _toDoListService = _services.GetRequiredService<IToDoListService>();
            _adminPosts = _services.GetRequiredService<AdminPostDatabase>();

            _adminPageBuilders = new()
            {
                { AdminPostType.GuildService, new GuildServiceStatsPageBuilder(
                    _services.GetRequiredService<GuildDatabase>(),
                    _services.GetRequiredService<IGuildService>()) },
                { AdminPostType.ServerStats, new ServerStatsPageBuilder(_logger) }
            };
        }

        private Timer _updateTimer;

        public async Task Init()
        {
            _updateTimer = new Timer(async x => await UpdatePosts(), null, TimeSpan.FromSeconds(1), TimeSpan.FromMinutes(5));
            await _logger.LogAsync(LogSeverity.Info, this, $"Initialized!");
        }

        public async Task UpdatePosts()
        {
            foreach (AdminPost post in _adminPosts.Values)
            {
                await UpdatePost(post);
            }
        }

        public async Task UpdatePost(AdminPostType type)
        {
            if (!_adminPosts.TryGetValue((int)type, out AdminPost post)) return;
            await UpdatePost(post);
        }

        public async Task UpdatePost(AdminPost post)
        {
            if (!_adminPageBuilders.TryGetValue(post.Type, out ISettingsPageBuilder pageBuilder)) return;
            if (post.Channel == null || post.Guild == null) return;

            MessageContext message = await pageBuilder.BuildPage(null);
            message.component.WithButton("Refresh", $"botadmin-refresh-{post.Type}");

            try
            {
                //Try to update message
                await post.Message.ModifyAsync(x => { x.Embed = message.embed; x.Components = message.component.Build(); });
            }
            catch
            {
                //Post new message
                post.Message = await post.Channel.SendMessageAsync(embed: message.embed, components: message.component.Build());
                post.MessageID = post.Message.Id;
                _adminPosts.Save((int)post.Type);
            }
        }

        public async Task SetPostChannel(AdminPostType type, IGuild guild, ITextChannel channel)
        {
            if (!_adminPosts.TryGetValue((int)type, out AdminPost post))
            {
                post = new(type);
                _adminPosts.Add((int)type, post);
            }

            post.Guild = guild;
            post.GuildID = guild.Id;
            post.Channel = channel;
            post.ChannelID = channel.Id;
            
            if (post.MessageID != null)
            {
                if (post.Message != null) await post.Message.DeleteAsync();
                post.Message = null;
                post.MessageID = null;
            }

            _adminPosts.Save((int)post.Type);

            await UpdatePost(type);
        }

        public async Task RefreshList(ulong guildID, string listName)
        {
            await _toDoListService.UpdateToDoList(guildID, listName);
        }

        public async Task Shutdown()
        {
            //Update stats to offline, leave embeds to show offline
            foreach (AdminPost post in _adminPosts.Values)
            {
                if (post.Message == null) continue;

                List<Embed> embeds = post.Message.Embeds.Select(x => (Embed)x).ToList();
                embeds.Add(new EmbedBuilder().WithTitle($"Bot Offline").WithColor(Color.Red).Build());
                await post.Message.ModifyAsync(x => { x.Embeds = embeds.ToArray(); x.Components = new ComponentBuilder().Build(); });
            }
        }
    }
}
