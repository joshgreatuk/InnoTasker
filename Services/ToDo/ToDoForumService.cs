﻿using Discord;
using InnoTasker.Data;
using InnoTasker.Data.ToDo;
using InnoTasker.Services.Interfaces;
using InnoTasker.Services.Interfaces.ToDo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnoTasker.Services.ToDo
{
    public class ToDoForumService : InnoServiceBase, IToDoForumService
    {
        private readonly IGuildService _guildService;

        public ToDoForumService(ILogger logger, IGuildService guildService) : base(logger)
        {
            _guildService = guildService;
        }

        public async Task InitService()
        {
            //Update forum status messages
            await _logger.LogAsync(LogSeverity.Info, this, $"Started initialization");
            foreach (ToDoItem item in (await _guildService.GetGuildDataList()).SelectMany(x => x.Lists.SelectMany(x => x.Items)))
            {
                if (item.SorryMessage != null)
                {
                    await item.SorryMessage.DeleteAsync();
                    item.SorryMessage = null;
                    item.SorryMessageID = null;
                }
            }
            await _logger.LogAsync(LogSeverity.Info, this, "Initialized!");
        }

        public async Task<bool> IsListForumEnabled(ulong guildID, string listName) =>
            await IsListForumEnabled(await _guildService.GetToDoList(guildID, listName));
        public async Task<bool> IsListForumEnabled(ToDoList list)
        {
            try
            {
                if (list.ForumChannel == null) throw new NullReferenceException();
                await list.ForumChannel.GetActiveThreadsAsync();
            }
            catch (Exception ex)
            {
                list.ForumChannel = null;
                return false;
            }
            return true;
        }

        public async Task<bool> DoesTaskPostExist(ToDoItem item)
        {
            try
            {
                if (item.ForumPost == null) throw new NullReferenceException();
                item.ForumPost.EnterTypingState().Dispose();
            }
            catch
            {
                return false;
            }
            return true;
        }

        public async Task CreateTaskPost(ToDoList list, ToDoItem item)
        {
            if (await DoesTaskPostExist(item)) return;
            if (!await IsListForumEnabled(list)) return;

            item.ForumPost = await list.ForumChannel.CreatePostAsync($"#{item.ID} | {item.Name}", embed: await CreateStatusMessage(item));
            item.ForumPostID = item.ForumPost.Id;
            item.StatusMessage = await item.ForumPost.ModifyMessageAsync(item.ForumPost.GetMessagesAsync(1).FirstAsync().Result.First().Id, x => x.Content = "");
            item.StatusMessageID = item.StatusMessage.Id;
            await item.StatusMessage.PinAsync();
        }

        public async Task CompleteTaskPost(ToDoItem item)
        {
            if (!await DoesTaskPostExist(item)) return;

            await item.ForumPost.ModifyAsync(x => { x.Locked = true; x.Archived = true; });
        }

        public async Task UnCompleteTaskPost(ToDoItem item)
        {
            if (!await DoesTaskPostExist(item)) return;

            await item.ForumPost.ModifyAsync(x => { x.Locked = false; x.Archived = false; });
        }

        public async Task AddUserTaskPost(ToDoItem item, IGuildUser user)
        {
            if (!await DoesTaskPostExist(item)) return;

            await item.ForumPost.AddUserAsync(user);
        }

        public async Task RemoveUserTaskPost(ToDoItem item, IGuildUser user)
        {
            if (!await DoesTaskPostExist(item)) return;

            await item.ForumPost.RemoveUserAsync(user);
        }

        public async Task UpdateStatusMessage(ToDoItem item)
        {
            if (!await DoesTaskPostExist(item)) return;

            Embed statusMessage = await CreateStatusMessage(item);

            try
            {
                await item.StatusMessage.ModifyAsync(x => x.Embed = statusMessage);
            }
            catch //Message doesnt exist, create and pin it
            {
                item.StatusMessage = await item.ForumPost.SendMessageAsync(embed: statusMessage);
                item.StatusMessageID = item.StatusMessage.Id;
                await item.StatusMessage.PinAsync();
            }
        }

        public async Task<Embed> CreateStatusMessage(ToDoItem item, Color? colour=null)
        {
            if (colour == null) colour = Color.Default;

            List<string> statusFields = new()
            {
                $"ID: {item.ID}",
                $"Name: {item.Name}",
                $"Stages: {string.Join(", ", item.Stages)}",
                $"Categories: {string.Join(", ", item.Categories)}",
                $"Assigned users: {string.Join(", ", item.AssignedUsers.Select(x => MentionUtils.MentionUser(x)))}",
            };

            return new EmbedBuilder()
                .WithTitle($"#{item.ID} | {item.Name}")
                .WithDescription(string.Join("\n", statusFields))
                .WithColor((Color)colour)
                .Build();
        }

        public async Task<IUserMessage> ProcessUpdateMessages(ToDoItem item, Color? colour=null)
        {
            if (!await DoesTaskPostExist(item)) return null;

            if (colour == null) colour = Color.Default;

            //Get message from ItemUpdate.GetMessage()
            IUserMessage message = await item.ForumPost.SendMessageAsync(embed: new EmbedBuilder()
                .WithTitle($"Task Updates:")
                .WithDescription(string.Join("\n", item.ItemUpdateQueue.Select(x => x.ToString())))
                .WithColor((Color)colour)
                .Build());
            
            item.ItemUpdateQueue.Clear();
            return message;
        }

        public async Task Shutdown()
        {
            await _logger.LogAsync(LogSeverity.Info, this, $"Shutting down");
            foreach (ToDoItem item in _guildService.GetGuildDataList()
                .Result.SelectMany(x => x.Lists.Where(x => IsListForumEnabled(x).Result)
                .SelectMany(x => x.Items.Where(x => x.ForumPost != null))))
            {
                if (item.ItemUpdateQueue.Count > 0 ) await ProcessUpdateMessages(item); //Hack so that only bot offline message is red
                item.ItemUpdateQueue.Enqueue(new ItemUpdate(ItemUpdateType.BotShutdown, string.Empty));
                item.SorryMessage = await ProcessUpdateMessages(item, Color.Red);
                item.SorryMessageID = item.SorryMessage.Id;
            }
            await _logger.LogAsync(LogSeverity.Info, this, "Apologized in forum posts");
        }
    }
}
