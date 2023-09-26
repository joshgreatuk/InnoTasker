using Discord;
using Discord.Net;
using Discord.Rest;
using InnoTasker.Data;
using InnoTasker.Data.ToDo;
using InnoTasker.Services.Interfaces;
using InnoTasker.Services.Interfaces.ToDo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace InnoTasker.Services.ToDo
{
    public class ToDoListService : InnoServiceBase, IToDoListService
    {
        private readonly IGuildService _guildService;
        private readonly IToDoForumService _toDoForumService;

        public ToDoListService(ILogger logger, IGuildService guildService, IToDoForumService toDoForumService) : base(logger)
        {
            _guildService = guildService;
            _toDoForumService = toDoForumService;
        }

        public async Task InitService()
        {
            //Update the todo list and message
            await _logger.LogAsync(LogSeverity.Info, this, $"Started initialization");
            foreach (GuildData guild in await _guildService.GetGuildDataList())
            {
                foreach (ToDoList list in guild.Lists)
                {
                    await UpdateToDoList(guild.ID, list);
                }
            }
            await _logger.LogAsync(LogSeverity.Info, this, "Initialized!");
        }

        public async Task UpdateToDoList(ulong guildID, string listName, Dictionary<string, string> renamedCategories = null, Dictionary<string, string> renamedStages = null) => 
            await UpdateToDoList(guildID, await _guildService.GetToDoList(guildID, listName), renamedCategories, renamedStages);
        public async Task UpdateToDoList(ulong guildID, ToDoList list, Dictionary<string, string> renamedCategories=null, Dictionary<string, string> renamedStages=null)
        {
            //Check for removed and renamed categories, check renamed before removed
            foreach (ToDoItem item in list.Items)
            {
                bool itemChanged = false;
                Queue<string> toRemove = new();

                if (renamedCategories != null)
                {
                    for (int i = 0; i < item.Categories.Count; i++)
                    {
                        if (renamedCategories.TryGetValue(item.Categories[i], out string newCat))
                        {
                            item.ItemUpdateQueue.Enqueue(new ItemUpdate(ItemUpdateType.CategoryRenamed, $"{item.Categories[i]}:{newCat}"));
                            item.Categories[i] = newCat;
                            itemChanged = true;
                        }

                        if (!list.Categories.Contains(item.Categories[i]))
                        {
                            item.ItemUpdateQueue.Enqueue(new ItemUpdate(ItemUpdateType.CategoryRemoved, item.Categories[i]));
                            toRemove.Enqueue(item.Categories[i]);
                            itemChanged = true;
                        }
                    }

                    foreach (string remove in toRemove)
                    {
                        item.Categories.Remove(remove);
                    }

                    toRemove.Clear();
                }

                if (renamedStages != null)
                {
                    for (int i = 0; i < item.Stages.Count; i++)
                    {
                        if (renamedStages.TryGetValue(item.Stages[i], out string newStage))
                        {
                            item.ItemUpdateQueue.Enqueue(new ItemUpdate(ItemUpdateType.StageRenamed, $"{item.Categories[i]}:{newStage}"));
                            item.Stages[i] = newStage;
                            itemChanged = true;
                        }

                        if (!list.Stages.Contains(item.Stages[i]))
                        {
                            item.ItemUpdateQueue.Enqueue(new ItemUpdate(ItemUpdateType.StageRemoved, item.Stages[i]));
                            toRemove.Enqueue(item.Stages[i]);
                            itemChanged = true;
                        }
                    }

                    foreach (string remove in toRemove)
                    {
                        item.Stages.Remove(remove);
                    }
                }

                if (item.CachedToDoEntry == null || itemChanged)
                {
                    await UpdateToDoItem(guildID, list, item, false);
                }
            }

            await _guildService.SaveGuild(guildID);
            await UpdateToDoListMessage(list);
        }

        public async Task UpdateToDoListMessage(ulong guildID, string listName) =>
            await UpdateToDoListMessage(await _guildService.GetToDoList(guildID, listName));
        public async Task UpdateToDoListMessage(ToDoList list)
        {
            //Check that the channels still exist
            if (list.ListChannel == null) return;
            try
            {
                list.ListChannel.EnterTypingState().Dispose();
            }
            catch (HttpException ex)
            {
                list.ListChannel = null;
                await _logger.LogAsync(LogSeverity.Warning, this, $"Couldnt enter typing state, channel must not exist", ex);
                return;
            }

            List<Embed> listEmbeds = await CreateToDoEmbed(list);

            if (list.Message != null && list.MessageChannel != null && list.MessageChannel != list.ListChannel)
            {
                //Delete the old message
                await list.MessageChannel.DeleteMessageAsync(list.Message);
                list.Message = null;
                list.MessageID = null;
                list.MessageChannel = null;
                list.MessageChannelID = null;
            }

            //Check if the message exists
            try 
            {
                if (list.Message == null) throw new NullReferenceException();
                await list.ListChannel.GetMessageAsync(list.Message.Id);
            }
            catch (Exception ex)
            {
                //Create the message then return
                list.Message = await list.ListChannel.SendMessageAsync(embeds: listEmbeds.ToArray());
                list.MessageID = list.Message.Id;
                list.MessageChannel = list.ListChannel;
                list.MessageChannelID = list.ListChannel.Id;
                return;
            }

            //Modify the message, it exists!
            await list.Message.ModifyAsync(x => x.Embeds =  listEmbeds.ToArray());
        }

        public async Task UpdateToDoItem(ulong guildID, string listName, int itemID) => 
            await UpdateToDoItem(guildID, await _guildService.GetToDoList(guildID, listName), itemID);
        public async Task UpdateToDoItem(ulong guildID, ToDoList list, int itemID) => 
            await UpdateToDoItem(guildID, list, await list.GetToDoItem(itemID));
        public async Task UpdateToDoItem(ulong guildID, ToDoList list, ToDoItem item, bool updateMessage = true)
        {
            //Create to-do item message
            List<string> entries = new();
            entries.Add($"#{item.ID}");
            entries.Add($"{item.Name}");
            entries.Add($"{string.Join(", ", item.Stages)}");
            entries.Add($"{string.Join(", ", item.Categories)}");
            if (await _toDoForumService.IsListForumEnabled(list) && item.ForumPost != null)
            {
                entries.Add($"{MentionUtils.MentionChannel(item.ForumPost.Id)}");
            }

            item.CachedToDoEntry = string.Join(" | ", entries);

            if (item.IsComplete)
            {
                item.CachedToDoEntry = "~~" + item.CachedToDoEntry + "~~";
            }

            await _guildService.SaveGuild(guildID);

            if (updateMessage) await UpdateToDoListMessage(list);

            if (await _toDoForumService.IsListForumEnabled(list) && item.ItemUpdateQueue.Count > 0 )
            {
                await _toDoForumService.ProcessUpdateMessages(item);
            }
        }

        public async Task<List<Embed>> CreateToDoEmbed(ToDoList list)
        {
            List<string> infoItems = new()
            {
                $"**Name:** {list.Name}",
                $"**Stages:** {string.Join(", ", list.Stages)}",
                $"**Categories:** {string.Join(", ", list.Categories)}",
                $"**Item count:** {list.Items.Count}",
                $"**Completed items:** {list.Items.Count(x => x.IsComplete)}"
            };

            //Default sorting is by:
            //1. Completed
            //2. Stage
            //3. Category
            //4. ID

            list.Items.OrderBy(x => x.IsComplete).ThenBy(x => x.Stages.OrderBy(x => list.Stages.IndexOf(x)).First())
                .ThenBy(x => x.Categories.OrderBy(x => list.Categories.IndexOf(x)).First()).ThenBy(x => x.ID);

            List<string> itemMessages = list.Items.Select(x => x.CachedToDoEntry).ToList();

            //List info as first embed
            //Subsequent embeds must be split by items limit
            List<Embed> embeds = new()
            {
                new EmbedBuilder()
                .WithTitle($"To-Do list {list.Name} info")
                .WithDescription(string.Join("\n", infoItems))
                .WithCurrentTimestamp()
                .Build(),

                new EmbedBuilder()
                .WithTitle($"To-Do list {list.Name}")
                .WithDescription(string.Join("\n", itemMessages))
                .WithCurrentTimestamp()
                .Build()
            };

            return embeds;
        }

        public async Task AddToDoList(ulong guildID, ToDoList list)
        {
            await _guildService.AddNewList(guildID, list);
            await UpdateToDoList(guildID, list);
        }

        public async Task DeleteToDoList(ulong guildID, string toDoName)
        {
            ToDoList list = await _guildService.GetToDoList(guildID, toDoName);
            await _guildService.RemoveList(guildID, toDoName);

            if (list.Message == null) return;
            Embed statusEmbed = new EmbedBuilder()
                .WithTitle("List Archived")
                .WithDescription("This list can no longer be updated/used, feel free to delete the message/channels")
                .WithCurrentTimestamp()
                .Build();
            await AddStatusMessage(list.Message, statusEmbed);
        }

        public async Task AddToDoItem(ulong guildID, string listName, ToDoItem item) =>
            await AddToDoItem(guildID, await _guildService.GetToDoList(guildID, listName), item);
        public async Task AddToDoItem(ulong guildID, ToDoList list, ToDoItem item)
        {
            //Assign ID
            item.ID = list.CurrentID;
            list.CurrentID++;
            list.Items.Add(item);
            await _logger.LogAsync(LogSeverity.Debug, this, $"Item added to list {guildID}/{list.Name}/({item.ID}){item.Name}");
            await _guildService.SaveGuild(guildID);

            await UpdateToDoItem(guildID, list, item);
        }

        public async Task RemoveToDoItem(ulong guildID, string listName, int itemID) =>
            await RemoveToDoItem(guildID, await _guildService.GetToDoList(guildID, listName), itemID);
        public async Task RemoveToDoItem(ulong guildID, string listName, ToDoItem item) =>
            await RemoveToDoItem(guildID, await _guildService.GetToDoList(guildID, listName), item);
        public async Task RemoveToDoItem(ulong guildID, ToDoList list, int itemID) =>
            await RemoveToDoItem(guildID, list, await list.GetToDoItem(itemID));
        public async Task RemoveToDoItem(ulong guildID, ToDoList list, ToDoItem item)
        {
            if (list.Items.Contains(item))
            {
                list.Items.Remove(item);
                await _logger.LogAsync(LogSeverity.Debug, this, $"Item removed from list {guildID}/{list.Name}/({item.ID}){item.Name}");
                await _guildService.SaveGuild(guildID);

                await UpdateToDoListMessage(list);
            }
        }

        public async Task Shutdown()
        {
            await _logger.LogAsync(LogSeverity.Info, this, $"Shutting down");
            //Leave a nice embed message for users
            Embed messageEmbed = new EmbedBuilder().WithTitle("Sorry!")
                .WithDescription("InnoTasker is currently down for maintenence. Sorry for the inconvenience <3")
                .WithCurrentTimestamp()
                .Build();
            foreach (RestUserMessage listMessage in await _guildService.GetListMessages())
            {
                await AddStatusMessage(listMessage, messageEmbed);
            }
            await _logger.LogAsync(LogSeverity.Info, this, $"Apologized in to-do lists");
        }

        public async Task AddStatusMessage(IUserMessage message, Embed embed)
        {
            await message.ModifyAsync(x =>
            {
                List<Embed> embeds = message.Embeds.Select(x => (Embed)x).ToList();
                embeds.Add(embed);
                x.Embeds = new Optional<Embed[]>(embeds.ToArray());
            });
        }
    }
}
