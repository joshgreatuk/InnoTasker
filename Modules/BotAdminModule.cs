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

        public async Task SetPostChannel(AdminPostType type, ITextChannel channel=null)
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
            await FollowupAsync(embeds: (await GenerateItemInfo(list, info)).Select(x => x.Build()).Take(10).ToArray());
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
            IGuild guild = _client.GetGuild(info.guildID);
            IGuildUser user = await guild.GetUserAsync(info.userID);

            EmbedBuilder embed = new EmbedBuilder()
                .WithTitle($"To-Do List (U/G/L) {info.userID}/{info.guildID}/{info.listName}")
                .WithDescription(string.Join("\n", await DumpListFields(guild, user, list)));
            
            return embed;
        }

        public async Task<List<EmbedBuilder>> GenerateItemInfo(ToDoList list, SupportInfo info) ///Currently supports 225 tasks
        {
            List<EmbedBuilder> embeds = new();
            embeds.Add(await GenerateListInfo(list, info));

            int currentPage = 1;
            int lastPage = (int)MathF.Ceiling(list.Items.Count / 25);
            EmbedBuilder currentBuilder = null;
            for (int i=0; i < list.Items.Count; i++)
            {
                if (i % 25 == 0)
                {
                    currentPage++;
                    if (currentBuilder != null) embeds.Add(currentBuilder);
                    currentBuilder = new EmbedBuilder().WithTitle($"List {list.Name} items page {currentPage}/{lastPage}");
                }

                ToDoItem item = list.Items[i];

                EmbedFieldBuilder newField = new EmbedFieldBuilder()
                    .WithName($"Item #{item.ID}({item.Name})")
                    .WithValue(string.Join("\n", await DumpItemFields(item)));
                currentBuilder.AddField(newField);
            }
            if (currentBuilder != null && !embeds.Contains(currentBuilder)) embeds.Add(currentBuilder);

            return embeds;
        }

        public async Task<List<string>> DumpListFields(IGuild guild, IGuildUser user, ToDoList list)
        {
            return new() 
            {
                $"**SupportID User:** {user.Id} - {user.DisplayName}({user.GlobalName})",
                $"**Guild:** {guild.Id} - {guild.Name}",
                $"**CurrentID:** {list.CurrentID}",
                $"**ListName:** {list.Name}",
                $"**ListChannel:** {list.ListChannelID} - {list.ListChannel.Name}",
                $"**CommandChannel:** {list.CommandChannelID} - {list.CommandChannel.Name}",
                $"**ForumChannel:** {list.ForumChannelID} - {list.ForumChannel.Name}",
                $"**Categories:** {list.Categories}",
                $"**Stages:** {list.Stages}",
                $"**MessageChannel:** {list.MessageChannelID} - {list.MessageChannel.Name}",
                $"**Message:** {list.MessageID}",
                $"**ItemCount (Total/Complete):** {list.Items.Count}/{list.Items.Count(x => x.IsComplete)}",
                $"**UserPermissions:** {list.UserPermissions}",
                $"**RolePermissions:** {list.RolePermissions}",
            };
        }

        public async Task<List<string>> DumpItemFields(ToDoItem item)
        {
            return new()
            {
                $"**ID:** {item.ID}",
                $"**Name:** {item.Name}",
                $"**Description:** {item.Description}",
                $"**Categories:** {item.Categories}",
                $"**Stages:** {item.Stages}",
                $"**IsComplete:** {item.IsComplete}",
                $"**AssignedUsers:** {item.AssignedUsers}",
                $"**ForumPost:** {item.ForumPostID} - {item.ForumPost.Name}",
                $"**StatusMessage:** {item.StatusMessageID}",
                $"**SorryMessage:** {item.SorryMessageID}",
                $"**CachedToDoEntry:** {item.CachedToDoEntry}",
                $"**ItemUpdateQueue:** {item.ItemUpdateQueue}",
            };
        }
    }
}
