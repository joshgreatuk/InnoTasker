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
    using Autocomplete;
    using global::InnoTasker.Services.Interfaces;
    using Preconditions;

    [Group("todo", "Commands relating to a to-do list")]
    public class ToDoModule : ToDoModuleBase
    {
        public ToDoModule(IGuildService guildService, IToDoUpdateService updateService) : base(guildService, updateService) { }

        [DoListUserPermissionCheck(ListUserPermissions.Editor)]
        public class ToDoModuleEditor : ToDoModuleBase
        {
            public ToDoModuleEditor(IGuildService guildService, IToDoUpdateService updateService) : base(guildService, updateService) { }

            [SlashCommand("create", "Creates a new to-do task")]
            public async Task CreateDoDoItem(string title, string description,
            [Autocomplete(typeof(CategoryAutocomplete))] string initialCategory = "",
            [Autocomplete(typeof(StageAutocomplete))] string initialStage = "")
            {
                ToDoItem item = new()
                {
                    Name = title,
                    Description = description,
                    Categories = initialCategory != "" ? new() { initialCategory } : new(),
                    Stages = initialStage != "" ? new() { initialStage } : new()
                };


                await _updateService.CreateToDoTask(Context.Guild.Id, listName, item);
            }

            [SlashCommand("delete", "Deletes a to-do task")]
            public async Task RemoveToDoItem([Autocomplete(typeof(ItemAutocomplete))] int taskID)
            {
                if (!await DoesTaskExist(taskID)) return;

                await _updateService.DeleteToDoTask(Context.Guild.Id, listName, taskID, Context.User.Id);
            }

            [SlashCommand("change-description", "Changes the description of a to-do task")]
            public async Task ChangeItemDescription([Autocomplete(typeof(ItemAutocomplete))] int taskID, string newDescription)
            {
                if (!await DoesTaskExist(taskID)) return;

                await _updateService.ChangeTaskDescription(Context.Guild.Id, listName, taskID, newDescription, Context.User.Id);
            }

            [SlashCommand("complete", "Marks a to-do task as complete")]
            public async Task CompleteItem([Autocomplete(typeof(ItemAutocomplete))] int taskID)
            {
                if (!await DoesTaskExist(taskID)) return;

                await _updateService.CompleteTask(Context.Guild.Id, listName, taskID, Context.User.Id);
            }

            [SlashCommand("uncomplete", "Unmarks a completed to-do task as complete")]
            public async Task UnCompleteItem([Autocomplete(typeof(ItemAutocomplete))] int taskID)
            {
                if (!await DoesTaskExist(taskID)) return;

                await _updateService.UnCompleteTask(Context.Guild.Id, listName, taskID, Context.User.Id);
            }

            [SlashCommand("add-category", "Adds a category to a to-do task")]
            public async Task AddCategory([Autocomplete(typeof(ItemAutocomplete))] int taskID,
                [Autocomplete(typeof(CategoryAutocomplete))] string category)
            {
                if (!await DoesTaskExist(taskID) || !await DoesCategoryExist(category)) return;

                await _updateService.TaskAddCategory(Context.Guild.Id, listName, taskID, category);
            }

            [SlashCommand("remove-category", "Removes a category from a to-do task")]
            public async Task RemoveCategory([Autocomplete(typeof(ItemAutocomplete))] int taskID,
                [Autocomplete(typeof(CategoryAutocomplete))] string category)
            {
                if (!await DoesTaskExist(taskID) || !await DoesCategoryExist(category)) return;

                await _updateService.TaskRemoveCategory(Context.Guild.Id, listName, taskID, category);
            }

            [SlashCommand("add-stage", "Adds a stage to a to-do task")]
            public async Task AddStage([Autocomplete(typeof(ItemAutocomplete))] int taskID,
                [Autocomplete(typeof(StageAutocomplete))] string stage)
            {
                if (!await DoesTaskExist(taskID) || !await DoesStageExist(stage)) return;

                await _updateService.TaskAddStage(Context.Guild.Id, listName, taskID, stage);
            }

            [SlashCommand("remove-stage", "Removes a stage from a to-do task")]
            public async Task RemoveStage([Autocomplete(typeof(ItemAutocomplete))] int taskID,
                [Autocomplete(typeof(StageAutocomplete))] string stage)
            {
                if (!await DoesTaskExist(taskID) || !await DoesStageExist(stage)) return;

                await _updateService.TaskRemoveStage(Context.Guild.Id, listName, taskID, stage);
            }
        }

        [DoListUserPermissionCheck(ListUserPermissions.User)]
        public class ToDoModuleUser : ToDoModuleBase
        {
            public ToDoModuleUser(IGuildService guildService, IToDoUpdateService updateService) : base(guildService, updateService) { }

            [SlashCommand("add-myself", "Adds yourself to a to-do task")]
            public async Task ItemAddMyself([Autocomplete(typeof(ItemAutocomplete))] int taskID)
            {
                if (!await DoesTaskExist(taskID)) return;

                await _updateService.TaskAddUser(Context.Guild.Id, listName, taskID, Context.Guild.GetUser(Context.User.Id));
            }

            [SlashCommand("remove-myself", "Removes yourself from a to-do task")]
            public async Task ItemRemoveMyself([Autocomplete(typeof(ItemAutocomplete))] int taskID)
            {
                if (!await DoesTaskExist(taskID)) return;

                await _updateService.TaskRemoveUser(Context.Guild.Id, listName, taskID, Context.Guild.GetUser(Context.User.Id));
            }
        }
    }
}
