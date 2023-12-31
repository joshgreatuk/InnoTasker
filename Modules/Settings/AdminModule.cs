﻿using Discord;
using Discord.Interactions;
using Discord.Rest;
using InnoTasker.Data;
using InnoTasker.Modules.Autocomplete;
using InnoTasker.Services.ToDo;
using InnoTasker.Services.Interfaces.ToDo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InnoTasker.Data.ToDo;
using InnoTasker.Services.Interfaces;
using System.Threading.Channels;

namespace InnoTasker.Modules.Settings
{
    using Preconditions;
    using Preconditions.Parameters;

    [Group("admin", "Server admin commands")]
    [RequireUserPermission(GuildPermission.Administrator)]
    public class AdminModule : InnoModuleBase
    {
        private readonly IGuildService _guildService;
        private readonly IToDoSettingsService _toDoSettingsService;
        private readonly IToDoListService _toDoListService;

        public AdminModule(IGuildService guildService, IToDoSettingsService toDoSettingsService, IToDoListService toDoListService)
        {
            _guildService = guildService;
            _toDoSettingsService = toDoSettingsService;
            _toDoListService = toDoListService;
        }

        [SlashCommand("openlistmenu", "Open the to-do list menu")]
        public async Task OpenToDoListMenu()
        {
            if (!await _toDoSettingsService.OpenToDoListPage(Context.Interaction))
            {
                await FollowupAsync(ephemeral: true, embed: new EmbedBuilder().WithTitle($"Error")
                    .WithDescription("Sorry, there was an error opening the to do list menu").Build());
            }
            success = false;
        }

        [SlashCommand("opensettingsmenu", "Open a to-do list's settings menu")]
        public async Task OpenToDoSettingsMenu(
            [Autocomplete(typeof(ListAutocomplete))][RequireListExists] string toDoName)
        {
            if (!await _toDoSettingsService.OpenSettings(Context.Interaction, toDoName, ToDoSettingsContext.Existing))
            {
                await FollowupAsync(ephemeral: true, embed: new EmbedBuilder().WithTitle($"Error")
                    .WithDescription("Sorry, there was an error opening the settings menu").Build());
            }
            success = false;
        }

        [SlashCommand("new-todolist", "Open the to-do list creation wizard")]
        public async Task NewToDoList(
            [RequireNamingLimits][RequireLengthLimit(LimitType.Name)] string toDoName)
        {
            if (!await _toDoSettingsService.OpenSettings(Context.Interaction, toDoName, ToDoSettingsContext.New))
            {
                await FollowupAsync(ephemeral: true, embed: new EmbedBuilder().WithTitle($"Error")
                    .WithDescription("Sorry, there was an error creating a new to-do list").Build());
            }
            success = false;
        }

        [SlashCommand("delete-todolist", "Delete a to-do list")]
        public async Task DeleteToDoList(
            [Autocomplete(typeof(ListAutocomplete))][RequireListExists] string toDoName)
        {
            await FollowupAsync(ephemeral: true, embed: new EmbedBuilder()
                .WithTitle("Are you sure?")
                .WithDescription($"Would you really like to delete list '{toDoName}'")
                .Build()
                , components: new ComponentBuilder()
                .WithButton("Yes", $"admin-deletelist-yes¬{toDoName}", ButtonStyle.Danger)
                .WithButton("No", "admin-deletelist-no", ButtonStyle.Secondary)
                .Build());
            success = false;
        }

        [ComponentInteraction("admin-deletelist-yes¬*", true)]
        public async Task DeleteToDoListAccept()
        {
            await Context.Interaction.DeleteOriginalResponseAsync();
            IComponentInteraction interaction = (IComponentInteraction)Context.Interaction;
            string toDoName = interaction.Data.CustomId.Split("¬").Last(); //TO-DO: Disallow to do list names with ¬ in them
            await _toDoListService.DeleteToDoList(Context.Guild.Id, toDoName);
            //Update a settings instance if one exists
            if (await _toDoSettingsService.InstanceExists(Context.Channel.Id)) await _toDoSettingsService.OpenToDoListPage(Context.Interaction);
        }

        [ComponentInteraction("admin-deletelist-no", true)]
        public async Task DeleteToDoListDeny()
        {
            //Delete response
            await Context.Interaction.DeleteOriginalResponseAsync();
            await FollowupAsync("Not Done!", ephemeral: true);
            success = false;
        }

        [RequireSettingsInstance]
        [SlashCommand("set-todo-channel", "Sets a to-do list channel, used with '/admin opensettingsmenu'")]
        public async Task SetToDoListChannel(ToDoListChannelType channelType, [ChannelTypes(new ChannelType[] { ChannelType.Text, ChannelType.Forum })] IChannel channel = null)
        {
            ToDoSettingsInstance instance = await _toDoSettingsService.GetSettingsInstance(Context.Channel.Id);

            switch (channelType)
            {
                case ToDoListChannelType.List:
                    {
                        if (channel != null && channel.GetChannelType() is not ChannelType.Text) break;
                        instance.toDoChannel = channel != null ? (ITextChannel)channel : null;
                        break;
                    }
                case ToDoListChannelType.Command:
                    {
                        if (channel != null && channel.GetChannelType() is not ChannelType.Text) break;
                        instance.toDoCommandChannel = channel != null ? (ITextChannel)channel : null;
                        break;
                    }
                case ToDoListChannelType.Forum:
                    {
                        if (channel != null && channel.GetChannelType() is not ChannelType.Forum) break;
                        instance.toDoForumChannel = channel != null ? (IForumChannel)channel : null;
                        break;
                    }
            }
            await UpdateCurrentSettingsPage();
        }

        [RequireSettingsInstance]
        [SlashCommand("unset-todo-channel", "Unsets a to-do channel, used with '/admin opensettingsmenu'")]
        public async Task UnsetToDoChannel(ToDoListChannelType channelType)
        {
            ToDoSettingsInstance instance = await _toDoSettingsService.GetSettingsInstance(Context.Channel.Id);

            switch (channelType)
            {
                case ToDoListChannelType.List:
                    {
                        instance.toDoChannel = null;
                        break;
                    }
                case ToDoListChannelType.Command:
                    {
                        instance.toDoCommandChannel = null;
                        break;
                    }
                case ToDoListChannelType.Forum:
                    {
                        instance.toDoForumChannel = null;
                        break;
                    }
            }
            await UpdateCurrentSettingsPage();
        }

        [RequireSettingsInstance]
        [SlashCommand("add-category", "Adds a task category, used with '/admin opensettingsmenu'")]
        public async Task AddCategory(string categoryName)
        {
            ToDoSettingsInstance instance = await _toDoSettingsService.GetSettingsInstance(Context.Channel.Id);
            if (instance.categoryList.Contains(categoryName))
            {
                await FollowupAsync($"Category '{categoryName}' already exists", ephemeral: true);
                success = false;
                return;
            }
            instance.categoryList.Add(categoryName);
            await UpdateCurrentSettingsPage();
        }

        [RequireSettingsInstance]
        [SlashCommand("remove-category", "Removes a task category, used with '/admin opensettingsmenu'")]
        public async Task RemoveCategory(
            [Autocomplete(typeof(SettingsCategoryAutocomplete))][RequireCategoryExists] string categoryName)
        {
            ToDoSettingsInstance instance = await _toDoSettingsService.GetSettingsInstance(Context.Channel.Id);
            if (!instance.categoryList.Contains(categoryName))
            {
                await FollowupAsync($"Category '{categoryName}' doesn't exist", ephemeral: true);
                success = false;
                return;
            }
            instance.categoryList.Remove(categoryName);

            await UpdateCurrentSettingsPage();
        }

        [RequireSettingsInstance]
        [SlashCommand("rename-category", "Renames a task category, used with '/admin opensettingsmenu'")]
        public async Task RenameCategory(
            [Autocomplete(typeof(SettingsCategoryAutocomplete))][RequireCategoryExists] string categoryName,
            [RequireNamingLimits][RequireLengthLimit(LimitType.Name)] string newName)
        {
            ToDoSettingsInstance instance = await _toDoSettingsService.GetSettingsInstance(Context.Channel.Id);
            if (!instance.categoryList.Contains(categoryName) || instance.categoryList.Contains(newName))
            {
                await FollowupAsync($"Category '{categoryName}' doesn't exist", ephemeral: true);
                success = false;
                return;
            }
            instance.categoryList[instance.categoryList.IndexOf(categoryName)] = newName;

            if (instance.categoriesRenamed.ContainsKey(categoryName))
            {
                instance.categoriesRenamed[categoryName] = newName;
            }
            else if (instance.categoriesRenamed.ContainsValue(categoryName))
            {
                string key = instance.categoriesRenamed.Where(x => x.Value == categoryName).First().Key;
                instance.categoriesRenamed[key] = newName;
            }
            else
            {
                instance.categoriesRenamed.Add(categoryName, newName);
            }

            await UpdateCurrentSettingsPage();
        }

        [RequireSettingsInstance]
        [SlashCommand("add-stage", "Adds a task stage, used with '/admin opensettingsmenu'")]
        public async Task AddStage(
            [RequireNamingLimits][RequireLengthLimit(LimitType.Name)] string stageName)
        {
            ToDoSettingsInstance instance = await _toDoSettingsService.GetSettingsInstance(Context.Channel.Id);
            if (instance.stageList.Contains(stageName))
            {
                await FollowupAsync($"Stage '{stageName}' already exist", ephemeral: true);
                success = false;
                return;
            }
            instance.stageList.Add(stageName);
            await UpdateCurrentSettingsPage();
        }

        [RequireSettingsInstance]
        [SlashCommand("remove-stage", "Removes a task stage, used with '/admin opensettingsmenu'")]
        public async Task RemoveStage(
            [Autocomplete(typeof(SettingsStageAutocomplete))][RequireStageExists] string stageName)
        {
            ToDoSettingsInstance instance = await _toDoSettingsService.GetSettingsInstance(Context.Channel.Id);
            if (!instance.stageList.Contains(stageName))
            {
                await FollowupAsync($"Stage '{stageName}' doesn't exist", ephemeral: true);
                success = false;
                return;
            }
            instance.stageList.Remove(stageName);

            await UpdateCurrentSettingsPage();
        }

        [SlashCommand("rename-stage", "Renames a task stage, used with '/admin opensettingsmenu'")]
        public async Task RenameStage(
            [Autocomplete(typeof(SettingsStageAutocomplete))][RequireStageExists] string stageName,
            [RequireNamingLimits][RequireLengthLimit(LimitType.Name)] string newName)
        {
            ToDoSettingsInstance instance = await _toDoSettingsService.GetSettingsInstance(Context.Channel.Id);
            if (!instance.stageList.Contains(stageName) || instance.stageList.Contains(newName))
            {
                await FollowupAsync($"Stage '{stageName}' doesn't exist");
                success = false;
                return;
            }
            instance.stageList[instance.stageList.IndexOf(stageName)] = newName;

            if (instance.stagesRenamed.ContainsKey(stageName))
            {
                instance.stagesRenamed[stageName] = newName;
            }
            else if (instance.stagesRenamed.ContainsValue(stageName))
            {
                string key = instance.stagesRenamed.Where(x => x.Value == stageName).First().Key;
                instance.stagesRenamed[key] = newName;
            }
            else
            {
                instance.stagesRenamed.Add(stageName, newName);
            }

            await UpdateCurrentSettingsPage();
        }

        [RequireSettingsInstance]
        [SlashCommand("set-user-permission", "Sets the to-do list permissions for a user, used with '/admin opensettingsmenu'")]
        public async Task SetUserPermission(IUser user, ListUserPermissions permissions)
        {
            ToDoSettingsInstance instance = await _toDoSettingsService.GetSettingsInstance(Context.Channel.Id);
            if (instance.userPermissions.ContainsKey(user.Id))
            {
                if (permissions is ListUserPermissions.None)
                {
                    instance.userPermissions.Remove(user.Id);
                }
                else
                {
                    instance.userPermissions[user.Id] = permissions;
                }
            }
            else if (permissions is not ListUserPermissions.None)
            {
                instance.userPermissions.Add(user.Id, permissions);
            }
            await UpdateCurrentSettingsPage();
        }

        [RequireSettingsInstance]
        [SlashCommand("set-role-permission", "Sets the to-do list permissions for a role, used with '/admin opensettingsmenu'")]
        public async Task SetRolePermission(IRole role, ListUserPermissions permissions)
        {
            ToDoSettingsInstance instance = await _toDoSettingsService.GetSettingsInstance(Context.Channel.Id);
            if (instance.rolePermissions.ContainsKey(role.Id))
            {
                if (permissions is ListUserPermissions.None)
                {
                    instance.rolePermissions.Remove(role.Id);
                }
                else
                {
                    instance.rolePermissions[role.Id] = permissions;
                }
            }
            else if (permissions is not ListUserPermissions.None)
            {
                instance.rolePermissions.Add(role.Id, permissions);
            }
            await UpdateCurrentSettingsPage();
        }

        [RequireSettingsInstance]
        [SlashCommand("sortby-set-main", "Set the main SortBy filter on the list, used with '/admin opensettingsmenu'")]
        public async Task SetMainSortBy(MainListSortType type)
        {
            ToDoSettingsInstance instance = await _toDoSettingsService.GetSettingsInstance(Context.Channel.Id);
            instance.mainSortBy.SortType = type;
            await UpdateCurrentSettingsPage();
        }

        [RequireSettingsInstance]
        [SlashCommand("sortby-set-sub", "Set the sub SortBy filter on the list, used with '/admin opensettingsmenu'")]
        public async Task SetSubSortBy(SubListSortType type) 
        {
            ToDoSettingsInstance instance = await _toDoSettingsService.GetSettingsInstance(Context.Channel.Id);
            instance.subSortBy.SortType = type;
            await UpdateCurrentSettingsPage();
        }

        public enum SortByRef { Main, Sub }

        [RequireSettingsInstance]
        [SlashCommand("sortby-toggle-direction", "Toggle the direction of a SortBy filter, used with '/admin opensettingsmenu'")]
        public async Task ToggleSortByDirection(SortByRef type)
        {
            ToDoSettingsInstance instance = await _toDoSettingsService.GetSettingsInstance(Context.Channel.Id);
            if (type is SortByRef.Main)
            {
                instance.mainSortBy.Direction = instance.mainSortBy.Direction is ListSortDirection.Ascending 
                    ? ListSortDirection.Descending : ListSortDirection.Ascending;
            }
            else if (type is SortByRef.Sub)
            {
                instance.subSortBy.Direction = instance.subSortBy.Direction is ListSortDirection.Ascending 
                    ? ListSortDirection.Descending : ListSortDirection.Ascending;
            }
            await UpdateCurrentSettingsPage();
        }

        public async Task UpdateCurrentSettingsPage()
        {
            if (await _toDoSettingsService.InstanceExists(Context.Channel.Id))
            {
                MessageContext updatedPage = await _toDoSettingsService.GetCurrentSettingsPage(Context.Channel.Id);
                await _toDoSettingsService.UpdateInstance(Context.Interaction, updatedPage);
            }
        }
    }
}
