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

            }
            await _logger.LogAsync(LogSeverity.Info, this, "Initialized!");
        }

        public async Task UpdateToDoList(ulong guildID, string listName, Dictionary<string, string> renamedCategories = null, Dictionary<string, string> renamedStages = null) => 
            await UpdateToDoList(await _guildService.GetToDoList(guildID, listName), renamedCategories, renamedStages);
        public async Task UpdateToDoList(ToDoList list, Dictionary<string, string> renamedCategories=null, Dictionary<string, string> renamedStages=null)
        {
            //Check for removed and renamed categories, check renamed before removed

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
                list.MessageChannel = null;
            }

            //Check if the message exists
            try 
            {
                if (list.Message == null) throw new NullReferenceException();
                await list.ListChannel.GetMessageAsync(list.Message.Id);
            }
            catch
            {
                //Create the message then return
                list.Message = await list.ListChannel.SendMessageAsync(embeds: listEmbeds.ToArray());
                list.MessageChannel = list.ListChannel;
                return;
            }

            //Modify the message, it exists!
            await list.Message.ModifyAsync(x => x.Embeds =  listEmbeds.ToArray());
        }

        public async Task UpdateToDoItem(ulong guildID, string listName, int itemID) => 
            await UpdateToDoItem(guildID, await _guildService.GetToDoList(guildID, listName), itemID);
        public async Task UpdateToDoItem(ulong guildID, ToDoList list, int itemID) => 
            await UpdateToDoItem(guildID, list, await list.GetToDoItem(itemID));
        public async Task UpdateToDoItem(ulong guildID, ToDoList list, ToDoItem item)
        {
            //Create to-do item message

            await _guildService.SaveGuild(guildID);
            await UpdateToDoListMessage(list);

            if (await _toDoForumService.IsListForumEnabled(list))
            {
                
            }
        }

        public async Task<List<Embed>> CreateToDoEmbed(ToDoList list)
        {
            List<Embed> embeds = new();

            return embeds;
        }

        public async Task AddToDoList(ulong guildID, ToDoList list)
        {
            await _guildService.AddNewList(guildID, list);
            await UpdateToDoList(list);
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
                Embed[] embeds = new Embed[x.Embeds.Value.Length + 1];
                x.Embeds.Value.CopyTo(embeds, 0);
                embeds[embeds.Length - 1] = embed;
                x.Embeds = new Optional<Embed[]>(embeds);
            });
        }
    }
}
