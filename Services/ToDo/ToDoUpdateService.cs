using Discord;
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
    public class ToDoUpdateService : InnoServiceBase, IToDoUpdateService
    {
        private readonly IGuildService _guildService;
        private readonly IToDoListService _toDoListService;
        private readonly IToDoForumService _toDoForumService;

        public ToDoUpdateService(ILogger logger, IGuildService guildService, IToDoListService toDoListService, IToDoForumService toDoForumService) : base(logger)
        {
            _guildService = guildService;
            _toDoListService = toDoListService;
            _toDoForumService = toDoForumService;
        }

        #region Task Methods
        public async Task CreateToDoTask(ulong guildID, string listName, ToDoItem newItem)
        {
            await _toDoListService.AddToDoItem(guildID, listName, newItem);
        }

        public async Task DeleteToDoTask(ulong guildID, string listName, int taskID, ulong user)
        {
            ToDoList list = await GetList(guildID, listName);
            ToDoItem item = await list.GetToDoItem(taskID);
            await _toDoListService.RemoveToDoItem(guildID, list, item);

            if (await _toDoForumService.IsListForumEnabled(list))
            {
                item.ItemUpdateQueue.Enqueue(new ItemUpdate(ItemUpdateType.TaskRemoved, user.ToString()));
                await _toDoForumService.UpdateStatusMessage(item);
                await _toDoForumService.ProcessUpdateMessages(item);
                await _toDoForumService.CompleteTaskPost(item);
            }
        }

        public async Task ChangeTaskDescription(ulong guildID, string listName, int taskID, string newDescription, ulong user)
        {
            ToDoList list = await GetList(guildID, listName);
            ToDoItem item = await list.GetToDoItem(taskID);

            item.Description = newDescription;
            
            if (await _toDoForumService.IsListForumEnabled(list))
            {
                item.ItemUpdateQueue.Enqueue(new ItemUpdate(ItemUpdateType.DescriptionUpdate, user.ToString()));
                await _toDoForumService.UpdateStatusMessage(item);
                await _toDoForumService.ProcessUpdateMessages(item);
            }
        }

        public async Task CompleteTask(ulong guildID, string listName, int taskID, ulong user)
        {
            ToDoList list = await GetList(guildID, listName);
            ToDoItem item = await list.GetToDoItem(taskID);

            item.IsComplete = true;

            if (await _toDoForumService.IsListForumEnabled(list))
            {
                item.ItemUpdateQueue.Enqueue(new ItemUpdate(ItemUpdateType.Completed, user.ToString()));
                await _toDoForumService.UpdateStatusMessage(item);
                await _toDoForumService.ProcessUpdateMessages(item);
                await _toDoForumService.CompleteTaskPost(item);
            }

            await _toDoListService.UpdateToDoItem(guildID, list, item);
        }

        public async Task UnCompleteTask(ulong guildID, string listName, int taskID, ulong user)
        {
            ToDoList list = await GetList(guildID, listName);
            ToDoItem item = await list.GetToDoItem(taskID);

            item.IsComplete = false;

            if (await _toDoForumService.IsListForumEnabled(list))
            {
                item.ItemUpdateQueue.Enqueue(new ItemUpdate(ItemUpdateType.UnCompleted, user.ToString()));
                await _toDoForumService.UpdateStatusMessage(item);
                await _toDoForumService.ProcessUpdateMessages(item);
                await _toDoForumService.UnCompleteTaskPost(item);
            }

            await _toDoListService.UpdateToDoItem(guildID, list, item);
        }

        public async Task TaskAddUser(ulong guildID, string listName, int taskID, IGuildUser user)
        {
            ToDoList list = await GetList(guildID, listName);
            ToDoItem item = await list.GetToDoItem(taskID);

            if (item.AssignedUsers.Contains(user.Id)) return;
            
            if (await _toDoForumService.IsListForumEnabled(list)) 
            {
                if (!await _toDoForumService.DoesTaskPostExist(item))
                {
                    await _toDoForumService.CreateTaskPost(list, item);
                }

                item.ItemUpdateQueue.Enqueue(new ItemUpdate(ItemUpdateType.UserAdded, user.Id.ToString()));
                await _toDoForumService.UpdateStatusMessage(item);
                await _toDoForumService.ProcessUpdateMessages(item);
                await _toDoForumService.AddUserTaskPost(item, user);
            }

            await _toDoListService.UpdateToDoItem(guildID, list, item);
        }

        public async Task TaskRemoveUser(ulong guildID, string listName, int taskID, IGuildUser user)
        {
            ToDoList list = await GetList(guildID, listName);
            ToDoItem item = await list.GetToDoItem(taskID);

            if (!item.AssignedUsers.Contains(user.Id)) return;

            if (await _toDoForumService.IsListForumEnabled(list))
            {
                if (!await _toDoForumService.DoesTaskPostExist(item))
                {
                    await _toDoForumService.CreateTaskPost(list, item);
                }

                item.ItemUpdateQueue.Enqueue(new ItemUpdate(ItemUpdateType.UserRemoved, user.Id.ToString()));
                await _toDoForumService.UpdateStatusMessage(item);
                await _toDoForumService.RemoveUserTaskPost(item, user);
                await _toDoForumService.ProcessUpdateMessages(item);   
            }

            await _toDoListService.UpdateToDoItem(guildID, list, item);
        }

        public async Task TaskAddStage(ulong guildID, string listName, int taskID, string stage)
        {
            ToDoList list = await GetList(guildID, listName);
            ToDoItem item = await list.GetToDoItem(taskID);

            if (!list.Stages.Contains(stage)) return;
            if (item.Stages.Contains(stage)) return;
            item.Stages.Add(stage);

            if (await _toDoForumService.IsListForumEnabled(list))
            {
                item.ItemUpdateQueue.Enqueue(new ItemUpdate(ItemUpdateType.StageAdded, stage));
                await _toDoForumService.UpdateStatusMessage(item);
                await _toDoForumService.ProcessUpdateMessages(item);
            }

            await _toDoListService.UpdateToDoItem(guildID, list, item);
        }

        public async Task TaskRemoveStage(ulong guildID, string listName, int taskID, string stage)
        {
            ToDoList list = await GetList(guildID, listName);
            ToDoItem item = await list.GetToDoItem(taskID);

            if (!item.Stages.Contains(stage)) return;
            item.Stages.Remove(stage);

            if (await _toDoForumService.IsListForumEnabled(list))
            {
                item.ItemUpdateQueue.Enqueue(new ItemUpdate(ItemUpdateType.StageRemoved, stage));
                await _toDoForumService.UpdateStatusMessage(item);
                await _toDoForumService.ProcessUpdateMessages(item);
            }

            await _toDoListService.UpdateToDoItem(guildID, list, item);
        }

        public async Task TaskAddCategory(ulong guildID, string listName, int taskID, string category)
        {
            ToDoList list = await GetList(guildID, listName);
            ToDoItem item = await list.GetToDoItem(taskID);

            if (!list.Categories.Contains(category)) return;
            if (item.Categories.Contains(category)) return;

            item.Categories.Add(category);

            if (await _toDoForumService.IsListForumEnabled(list))
            {
                item.ItemUpdateQueue.Enqueue(new ItemUpdate(ItemUpdateType.CategoryAdded, category));
                await _toDoForumService.UpdateStatusMessage(item);
                await _toDoForumService.ProcessUpdateMessages(item);
            }

            await _toDoListService.UpdateToDoItem(guildID, list, item);
        }

        public async Task TaskRemoveCategory(ulong guildID, string listName, int taskID, string category)
        {
            ToDoList list = await GetList(guildID, listName);
            ToDoItem item = await list.GetToDoItem(taskID);

            if (!item.Categories.Contains(category)) return;

            item.Categories.Remove(category);

            if (await _toDoForumService.IsListForumEnabled(list))
            {
                item.ItemUpdateQueue.Enqueue(new ItemUpdate(ItemUpdateType.CategoryRemoved, category));
                await _toDoForumService.UpdateStatusMessage(item);
                await _toDoForumService.ProcessUpdateMessages(item);
            }

            await _toDoListService.UpdateToDoItem(guildID, list, item);
        }

        public async Task<string> GetListNameFromChannel(ulong guildID, ulong channelID)
        {
            ToDoList? list = await _guildService.GetToDoListFromChannel(guildID, channelID);
            return list != null ? list.Name : "";
        }
        #endregion
        #region Helper Methods
        public async Task<ToDoList> GetList(ulong guildID, string listName)
        {
            return await _guildService.GetToDoList(guildID, listName);
        }

        public async Task<ToDoItem> GetItem(ulong guildID, string listName, int itemID)
        {
            return await GetList(guildID, listName).Result.GetToDoItem(itemID);
        }
        #endregion
    }
}
