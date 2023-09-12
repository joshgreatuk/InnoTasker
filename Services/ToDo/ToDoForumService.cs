using Discord;
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
            foreach (GuildData guild in await _guildService.GetGuildDataList())
            {
                //Remove sorry messages
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
            catch
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

        public async Task CreateTaskPost(ToDoItem item)
        {
            if (await DoesTaskPostExist(item)) return;


        }

        public async Task CompleteTaskPost(ToDoItem item)
        {

        }

        public async Task UnCompleteTaskPost(ToDoItem item)
        {

        }

        public async Task UpdateStatusMessage(ToDoItem item)
        {

        }

        public async Task<IUserMessage> PostUpdateMessage(ToDoItem item, string title, string message)
        {
            throw new NotImplementedException();
        }

        public async Task Shutdown()
        {
            await _logger.LogAsync(LogSeverity.Info, this, $"Shutting down");
            foreach (ToDoItem item in _guildService.GetGuildDataList()
                .Result.SelectMany(x => x.Lists.Where(x => IsListForumEnabled(x).Result)
                .SelectMany(x => x.Items.Where(x => x.ForumPost != null))))
            {
                item.SorryMessage = await PostUpdateMessage(item, "Sorry!", "The bot is down for maintenence, sorry for the inconvenience <3");
            }
            await _logger.LogAsync(LogSeverity.Info, this, "Apologized in forum posts");
        }
    }
}
