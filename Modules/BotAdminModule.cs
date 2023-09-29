using Discord.Interactions;
using InnoTasker.Services.Interfaces.Admin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnoTasker.Modules
{
    using Discord;
    using Discord.WebSocket;
    using InnoTasker.Data.Admin;
    using InnoTasker.Data.ToDo;
    using InnoTasker.Services.Interfaces;
    using Preconditions;
    using System.ComponentModel;

    [DontAutoRegister]
    [RequireBotAdmin]
    public class BotAdminModule : InnoModuleBase
    {
        private readonly IAdminService _adminService;

        private DiscordSocketClient _client;
        private readonly IGuildService _guildService;

        public BotAdminModule(IAdminService adminService, DiscordSocketClient client, IGuildService guildService)
        {
            _adminService = adminService;
            _client = client;
            _guildService = guildService;
        }

        public async Task SetPostChannel(AdminPostType type, ITextChannel channel)
        {
            await _adminService.SetPostChannel(type, channel.Guild, channel);
        }

        public async Task RefreshList(string supportID)
        {
            SupportInfo info = Support.ParseSupportID(supportID);
            await _adminService.RefreshList(info.guildID, info.listName);
        }

        public async Task LookupList(string supportID)
        {
            SupportInfo info = Support.ParseSupportID(supportID);
            //Generate Embed with list info
            ToDoList list = await _guildService.GetToDoList(info.guildID, info.listName);
            success = false;
            await FollowupAsync(embed: (await GenerateListInfo(list, info)).Build());
        }

        public async Task LookupListDetailed(string supportID)
        {
            SupportInfo info = Support.ParseSupportID(supportID);
            //Generaete Embed with list info and also item info dump
            ToDoList list = await _guildService.GetToDoList(info.guildID, info.listName);
            success = false;
            EmbedBuilder embed = await GenerateListInfo(list, info);
            await FollowupAsync(embed: (await GenerateItemInfo(embed, list)).Build());
        }

        [ComponentInteraction("botadmin-refresh-*")]
        public async Task HandlePostUpdateRequest()
        {
            if (Context.Interaction is not IComponentInteraction interaction) return;

            AdminPostType postType = (AdminPostType)Enum.Parse(typeof(AdminPostType), interaction.Data.CustomId.Split("-").Last());
            await _adminService.UpdatePost(postType);
        }

        public async Task<EmbedBuilder> GenerateListInfo(ToDoList list, SupportInfo info)
        {
            EmbedBuilder embed = new();
            
            return embed;
        }

        public async Task<EmbedBuilder> GenerateItemInfo(EmbedBuilder embed, ToDoList list)
        {
            return embed;
        }

        public async Task<List<string>> DumpListFields(IGuild guild, IGuildUser user, ToDoList list)
        {
            return new() 
            { 

            };
        }

        public async Task<List<string>> DumpItemFields(ToDoItem item)
        {
            return new()
            {

            };
        }
    }
}
