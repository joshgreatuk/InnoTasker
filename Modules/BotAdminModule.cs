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
    using global::InnoTasker.Data.Admin;
    using global::InnoTasker.Data.ToDo;
    using global::InnoTasker.Services.Interfaces;
    using Preconditions;
    using System.ComponentModel;

    [DontAutoRegister]
    [RequireBotAdmin]
    [Group("botadmin", "These are bot admin commands, dont even try")]
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

        [SlashCommand("set-post-channel", "Users cannot run this")]
        public async Task SetPostChannel(AdminPostType type, ITextChannel channel=null)
        {
            await _adminService.SetPostChannel(type, channel.Guild, channel);
        }

        [SlashCommand("refresh-list", "Users cannot run this")]
        public async Task RefreshList(string supportID)
        {
            SupportInfo info = Support.ParseSupportID(supportID);
            await _adminService.RefreshList(info.guildID, info.listName);
        }

        [SlashCommand("lookup-list", "Users cannot run this")]
        public async Task LookupList(string supportID)
        {
            SupportInfo info = Support.ParseSupportID(supportID);
            //Generate Embed with list info
            ToDoList list = await _guildService.GetToDoList(info.guildID, info.listName);
            success = false;
            await FollowupAsync(embed: (await GenerateListInfo(list, info)).Build());
        }

        [SlashCommand("lookup-list-detailed", "Users cannot run this")]
        public async Task LookupListDetailed(string supportID)
        {
            SupportInfo info = Support.ParseSupportID(supportID);
            //Generaete Embed with list info and also item info dump
            ToDoList list = await _guildService.GetToDoList(info.guildID, info.listName);
            success = false;
            await FollowupAsync(embeds: (await GenerateItemInfo(list, info)).Select(x => x.Build()).Take(10).ToArray());
        }

        [SlashCommand("lookup-item", "Users cannot run this")]
        public async Task LookupItem(string supportID, int taskID)
        {
            SupportInfo info = Support.ParseSupportID(supportID);
            ToDoList list = await _guildService.GetToDoList(info.guildID, info.listName);
            success = false;

            ToDoItem? item = list.Items.FirstOrDefault(x => x.ID == taskID);
            if (item == null)
            {
                await FollowupAsync(embeds: new[] { new EmbedBuilder().WithTitle("Item doesnt exist").WithColor(Color.Red).Build() });
                return;
            }

            await FollowupAsync(embeds: new[] { (await GenerateListInfo(list, info)).Build(),
                new EmbedBuilder()
                .WithTitle($"Item #{item.ID} ({item.Name}) info")
                .WithDescription(string.Join("\n", DumpItemFields(item)))
                .Build() });
        }

        [ComponentInteraction("botadmin-refresh-*", true)]
        public async Task HandlePostUpdateRequest()
        {
            if (Context.Interaction is not IComponentInteraction interaction) return;

            AdminPostType postType = (AdminPostType)Enum.Parse(typeof(AdminPostType), interaction.Data.CustomId.Split("-").Last());
            await _adminService.UpdatePost(postType);

            success = false;
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

            int currentPage = 0;
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
                $"**Categories:** {String.Join(", ", list.Categories)}",
                $"**Stages:** {String.Join(", ", list.Stages)}",
                $"**MessageChannel:** {list.MessageChannelID} - {list.MessageChannel.Name}",
                $"**Messages:** {string.Join(", ", list.MessageIDs)}",
                $"**ItemCount (Total/Complete):** {list.Items.Count}/{list.Items.Count(x => x.IsComplete)}",
                $"**UserPermissions:** {String.Join(", ", list.UserPermissions)}",
                $"**RolePermissions:** {String.Join(", ", list.RolePermissions)}",
            };
        }

        public async Task<List<string>> DumpItemFields(ToDoItem item)
        {
            return new()
            {
                $"**ID:** {item.ID}",
                $"**Name:** {item.Name}",
                $"**Description:** {item.Description}",
                $"**Categories:** {String.Join(", ", item.Categories)}",
                $"**Stages:** {String.Join(", ", item.Stages)}",
                $"**IsComplete:** {item.IsComplete}",
                $"**AssignedUsers:** {String.Join(", ", item.AssignedUsers)}",
                $"**ForumPost:** {item.ForumPostID} - {(item.ForumPost != null ? item.ForumPost.Name : null)}",
                $"**StatusMessage:** {item.StatusMessageID}",
                $"**SorryMessage:** {item.SorryMessageID}",
                $"**CachedToDoEntry:** {item.CachedToDoEntry}",
                $"**ItemUpdateQueue:** {String.Join(", ", item.ItemUpdateQueue)}",
            };
        }
    }
}
