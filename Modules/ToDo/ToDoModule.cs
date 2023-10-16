using Discord.Interactions;
using InnoTasker.Services.Interfaces.ToDo;
using InnoTasker.Data.ToDo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnoTasker.Modules.ToDo
{
    using Autocomplete;
    using Discord;
    using global::InnoTasker.Services.Interfaces;
    using Preconditions;
    using Preconditions.Parameters;

    [Group("todo", "Commands relating to a to-do list")]
    public class ToDoModule : ToDoModuleBase
    {
        public ToDoModule(IGuildService guildService, IToDoUpdateService updateService) : base(guildService, updateService) { }
        
        [RequireUserPermission(GuildPermission.Administrator)]
        [DoListUserPermissionCheck(ListUserPermissions.Editor)]
        public class ToDoModuleEditor : ToDoModuleBase
        {
            public ToDoModuleEditor(IGuildService guildService, IToDoUpdateService updateService) : base(guildService, updateService) { }

            [SlashCommand("create", "Creates a new to-do task")]
            public async Task CreateDoDoItem(
                [RequireNamingLimits][RequireLengthLimit(LimitType.Name)] string title,
                [RequireLengthLimit(LimitType.Description)] string description,
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
            public async Task RemoveToDoItem(
                [Autocomplete(typeof(ItemAutocomplete))][RequireItemExists] int taskID)
            {
                await _updateService.DeleteToDoTask(Context.Guild.Id, listName, taskID, Context.User.Id);
            }

            [SlashCommand("change-description", "Changes the description of a to-do task")]
            public async Task ChangeItemDescription(
                [Autocomplete(typeof(ItemAutocomplete))][RequireItemExists] int taskID,
                [RequireLengthLimit(LimitType.Description)] string newDescription)
            {
                await _updateService.ChangeTaskDescription(Context.Guild.Id, listName, taskID, newDescription, Context.User.Id);
            }

            [SlashCommand("complete", "Marks a to-do task as complete")]
            public async Task CompleteItem(
                [Autocomplete(typeof(ItemAutocomplete))][RequireItemExists] int taskID)
            {
                await _updateService.CompleteTask(Context.Guild.Id, listName, taskID, Context.User.Id);
            }

            [SlashCommand("uncomplete", "Unmarks a completed to-do task as complete")]
            public async Task UnCompleteItem(
                [Autocomplete(typeof(ItemAutocomplete))][RequireItemExists] int taskID)
            {
                await _updateService.UnCompleteTask(Context.Guild.Id, listName, taskID, Context.User.Id);
            }

            [SlashCommand("add-category", "Adds a category to a to-do task")]
            public async Task AddCategory(
                [Autocomplete(typeof(ItemAutocomplete))][RequireItemExists] int taskID,
                [Autocomplete(typeof(CategoryAutocomplete))][RequireCategoryExists] string category)
            {
                await _updateService.TaskAddCategory(Context.Guild.Id, listName, taskID, category);
            }

            [SlashCommand("remove-category", "Removes a category from a to-do task")]
            public async Task RemoveCategory(
                [Autocomplete(typeof(ItemAutocomplete))][RequireItemExists] int taskID,
                [Autocomplete(typeof(CategoryAutocomplete))][RequireCategoryExists] string category)
            {
                await _updateService.TaskRemoveCategory(Context.Guild.Id, listName, taskID, category);
            }

            [SlashCommand("add-stage", "Adds a stage to a to-do task")]
            public async Task AddStage(
                [Autocomplete(typeof(ItemAutocomplete))][RequireItemExists] int taskID,
                [Autocomplete(typeof(StageAutocomplete))][RequireStageExists] string stage)
            {
                await _updateService.TaskAddStage(Context.Guild.Id, listName, taskID, stage);
            }

            [SlashCommand("remove-stage", "Removes a stage from a to-do task")]
            public async Task RemoveStage(
                [Autocomplete(typeof(ItemAutocomplete))][RequireItemExists] int taskID,
                [Autocomplete(typeof(StageAutocomplete))][RequireStageExists] string stage)
            {
                await _updateService.TaskRemoveStage(Context.Guild.Id, listName, taskID, stage);
            }
        }

        [RequireUserPermission(GuildPermission.Administrator)]
        [DoListUserPermissionCheck(ListUserPermissions.User)]
        public class ToDoModuleUser : ToDoModuleBase
        {
            public ToDoModuleUser(IGuildService guildService, IToDoUpdateService updateService) : base(guildService, updateService) { }

            [SlashCommand("add-myself", "Adds yourself to a to-do task")]
            public async Task ItemAddMyself(
                [Autocomplete(typeof(ItemAutocomplete))][RequireItemExists] int taskID)
            {
                await _updateService.TaskAddUser(Context.Guild.Id, listName, taskID, Context.Guild.GetUser(Context.User.Id));
            }

            [SlashCommand("remove-myself", "Removes yourself from a to-do task")]
            public async Task ItemRemoveMyself(
                [Autocomplete(typeof(ItemAutocomplete))][RequireItemExists] int taskID)
            {
                await _updateService.TaskRemoveUser(Context.Guild.Id, listName, taskID, Context.Guild.GetUser(Context.User.Id));
            }
        }
    }
}
