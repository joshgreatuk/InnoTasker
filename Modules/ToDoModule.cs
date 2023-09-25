using Discord.Interactions;
using InnoTasker.Services.Interfaces.ToDo;
using InnoTasker.Data.ToDo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnoTasker.Modules
{
    using InnoTasker.Services.Interfaces;
    using Preconditions;
    using System.Runtime.InteropServices;

    [Group("to-do", "Commands relating to a to-do list")]
    public class ToDoModule : ToDoModuleBase
    {
        public ToDoModule(IGuildService guildService, IToDoUpdateService updateService) : base(guildService, updateService) { }    

        [DoListUserPermissionCheck(ListUserPermissions.Editor)]
        [SlashCommand("create-item", "Creates a new to-do task")]
        public async Task CreateDoDoItem(string title, string description,
            string initialCategory, string initialStage)
        {
            ToDoItem item = new()
            {
                Name = title,
                Description = description,
                Categories = new() { initialCategory },
                Stages = new() { initialStage }
            };

            
            await _updateService.CreateToDoTask(Context.Guild.Id, listName, item);
        }

        [DoListUserPermissionCheck(ListUserPermissions.Editor)]
        [SlashCommand("delete-item", "Deletes a to-do task")]
        public async Task RemoveToDoItem(int taskID)
        {
            if (!await DoesTaskExist(taskID)) return;

            await _updateService.DeleteToDoTask(Context.Guild.Id, listName, taskID, Context.User.Id);
        }

        [DoListUserPermissionCheck(ListUserPermissions.Editor)]
        [SlashCommand("change-item-description", "Changes the description of a to-do task")]
        public async Task ChangeItemDescription(int taskID, string newDescription)
        {
            if (!await DoesTaskExist(taskID)) return;

            await _updateService.ChangeTaskDescription(Context.Guild.Id, listName, taskID, newDescription, Context.User.Id);
        }

        [DoListUserPermissionCheck(ListUserPermissions.Editor)]
        [SlashCommand("complete-item", "Marks a to-do task as complete")]
        public async Task CompleteItem(int taskID)
        {
            if (!await DoesTaskExist(taskID)) return;

            await _updateService.CompleteTask(Context.Guild.Id, listName, taskID, Context.User.Id);
        }

        [DoListUserPermissionCheck(ListUserPermissions.Editor)]
        [SlashCommand("uncomplete-item", "Unmarks a completed to-do task as complete")]
        public async Task UnCompleteItem(int taskID)
        {
            if (!await DoesTaskExist(taskID)) return;

            await _updateService.UnCompleteTask(Context.Guild.Id, listName, taskID, Context.User.Id);
        }

        [DoListUserPermissionCheck(ListUserPermissions.User)]
        [SlashCommand("item-add-myself", "Adds yourself to a to-do task")]
        public async Task ItemAddMyself(int taskID)
        {
            if (!await DoesTaskExist(taskID)) return;

            await _updateService.TaskAddUser(Context.Guild.Id, listName, taskID, Context.Guild.GetUser(Context.User.Id));
        }

        [DoListUserPermissionCheck(ListUserPermissions.User)]
        [SlashCommand("item-remove-myself", "Removes yourself from a to-do task")]
        public async Task ItemRemoveMyself(int taskID)
        {
            if (!await DoesTaskExist(taskID)) return;

            await _updateService.TaskRemoveUser(Context.Guild.Id, listName, taskID, Context.Guild.GetUser(Context.User.Id));
        }

        [DoListUserPermissionCheck(ListUserPermissions.Editor)]
        [SlashCommand("add-item-category", "Adds a category to a to-do task")]
        public async Task AddCategory(int taskID, string category)
        {
            if (!await DoesTaskExist(taskID) || !await DoesCategoryExist(category)) return;

            await _updateService.TaskAddCategory(Context.Guild.Id, listName, taskID, category);
        }

        [DoListUserPermissionCheck(ListUserPermissions.Editor)]
        [SlashCommand("remove-item-category", "Removes a category from a to-do task")]
        public async Task RemoveCategory(int taskID, string category)
        {
            if (!await DoesTaskExist(taskID) || !await DoesCategoryExist(category)) return;

            await _updateService.TaskRemoveCategory(Context.Guild.Id, listName, taskID, category);
        }

        [DoListUserPermissionCheck(ListUserPermissions.Editor)]
        [SlashCommand("add-item-stage", "Adds a stage to a to-do task")]
        public async Task AddStage(int taskID, string stage)
        {
            if (!await DoesTaskExist(taskID) || !await DoesStageExist(stage)) return;

            await _updateService.TaskAddStage(Context.Guild.Id, listName, taskID, stage);
        }

        [DoListUserPermissionCheck(ListUserPermissions.Editor)]
        [SlashCommand("remove-item-stage", "Removes a stage from a to-do task")]
        public async Task RemoveStage(int taskID, string stage)
        {
            if (!await DoesTaskExist(taskID) || !await DoesStageExist(stage)) return;

            await _updateService.TaskRemoveStage(Context.Guild.Id, listName, taskID, stage);
        }
    }
}
