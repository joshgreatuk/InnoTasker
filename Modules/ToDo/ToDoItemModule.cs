using Discord.Interactions;
using InnoTasker.Services.Interfaces;
using InnoTasker.Services.Interfaces.ToDo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InnoTasker.Modules.Autocomplete;
using InnoTasker.Modules.Preconditions.Parameters;

namespace InnoTasker.Modules.ToDo
{
    [Group("to-do-item", "Commands relating a task in it's forum post")]
    public class ToDoItemModule : ItemModuleBase
    {
        public ToDoItemModule(IGuildService guildService, IToDoUpdateService updateService) : base(guildService, updateService) { }

        [SlashCommand("change-description", "Change the description of this task")]
        public async Task ChangeDescription(
            [RequireLengthLimit(LimitType.Description)] string description)
        {
            await _updateService.ChangeTaskDescription(Context.Guild.Id, listName, item.ID, description, Context.User.Id);
        }

        [SlashCommand("complete-item", "Complete this task")]
        public async Task CompleteItem()
        {
            await _updateService.CompleteTask(Context.Guild.Id, listName, item.ID, Context.User.Id);
        }

        [SlashCommand("add-myself", "Add yourself to this task")]
        public async Task AddMyself()
        {
            await _updateService.TaskAddUser(Context.User.Id, listName, item.ID, Context.Guild.GetUser(Context.User.Id));
        }

        [SlashCommand("remove-myself", "Remove yourself from this task")]
        public async Task RemoveMyself()
        {
            await _updateService.TaskRemoveUser(Context.User.Id, listName, item.ID, Context.Guild.GetUser(Context.User.Id));
        }

        [SlashCommand("add-category", "Add a category to this task")]
        public async Task AddCategory(
            [Autocomplete(typeof(CategoryAutocomplete))] [RequireCategoryExists] [RequireItemCategoryAdded(true)] string category)
        {
            await _updateService.TaskAddCategory(Context.Guild.Id, listName, item.ID, category);
        }

        [SlashCommand("remove-category", "Removes a category from this task")]
        public async Task RemoveCategory(
            [Autocomplete(typeof(CategoryAutocomplete))] [RequireCategoryExists] [RequireItemCategoryAdded()] string category)
        {
            await _updateService.TaskRemoveCategory(Context.Guild.Id, listName, item.ID, category);
        }

        [SlashCommand("add-stage", "Adds a stage to this task")]
        public async Task AddStage(
            [Autocomplete(typeof(StageAutocomplete))] [RequireStageExists] [RequireItemStageAdded(true)] string stage)
        {
            await _updateService.TaskAddStage(Context.Guild.Id, listName, item.ID, stage);
        }

        [SlashCommand("remove-stage", "Removes a stage from this task")]
        public async Task RemoveStage(
            [Autocomplete(typeof(StageAutocomplete))] [RequireStageExists] [RequireItemStageAdded()] string stage)
        {
            await _updateService.TaskRemoveStage(Context.Guild.Id, listName, item.ID, stage);
        }
    }
}
