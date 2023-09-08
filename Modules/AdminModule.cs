using Discord;
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

namespace InnoTasker.Modules
{
    [Group("admin", "Server admin commands")]
    [RequireUserPermission(Discord.GuildPermission.Administrator)]
    public class AdminModule : InteractionModuleBase<SocketInteractionContext>
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
            await DeferAsync(true);
            if (await _toDoSettingsService.OpenToDoListPage(Context.Interaction))
            {
                await RespondAsync();
            }
            else
            {
                await RespondAsync(embed: new Discord.EmbedBuilder().WithTitle($"Error")
                    .WithDescription("Sorry, there was an error opening the to do list menu").Build());
            }
        }

        [SlashCommand("opensettingsmenu", "Open a to-do list's settings menu")]
        public async Task OpenToDoSettingsMenu([Autocomplete(typeof(ToDoListAutocomplete))] string toDoName)
        {
            await DeferAsync(true);
            if (await _toDoSettingsService.OpenSettings(Context.Interaction, toDoName, ToDoSettingsContext.Existing))
            {
                await RespondAsync(ephemeral: true);
            }
            else
            {
                await RespondAsync(ephemeral: true, embed: new Discord.EmbedBuilder().WithTitle($"Error")
                    .WithDescription("Sorry, there was an error opening the settings menu").Build());
            }
        }

        [SlashCommand("newtodolist", "Open the to-do list creation wizard")]
        public async Task NewToDoList(string toDoName)
        {
            await DeferAsync(true);
            if (await _toDoSettingsService.OpenSettings(Context.Interaction, toDoName, ToDoSettingsContext.New))
            {
                await RespondAsync(ephemeral: true);
            }
            else
            {
                await RespondAsync(ephemeral: true, embed: new Discord.EmbedBuilder().WithTitle($"Error")
                    .WithDescription("Sorry, there was an error creating a new to-do list").Build());
            }
        }

        [SlashCommand("deletetodolist", "Delete a to-do list")]
        public async Task DeleteToDoList([Autocomplete(typeof(ToDoListAutocomplete))] string toDoName)
        {
            await RespondAsync(embed: new EmbedBuilder()
                .WithTitle("Are you sure?")
                .WithDescription($"Would you really like to delete list '{toDoName}'")
                .Build()
                , components: new ComponentBuilder()
                .WithButton("Yes", $"admin-deletelist-yes¬{toDoName}", ButtonStyle.Danger)
                .WithButton("No", "admin-deletelist-no", ButtonStyle.Secondary)
                .Build());
        }

        [ComponentInteraction("admin-deletelist-yes¬*")]
        public async Task DeleteToDoListAccept()
        {
            IComponentInteraction interaction = (IComponentInteraction)Context.Interaction;
            string toDoName = interaction.Data.CustomId.Split("¬").Last(); //TO-DO: Disallow to do list names with ¬ in them
            await _toDoListService.DeleteToDoList(Context.Guild.Id, toDoName);
            //Update a settings instance if one exists
            if (await _toDoSettingsService.InstanceExists(Context.Channel.Id)) await _toDoSettingsService.OpenToDoListPage(Context.Interaction);
            await RespondAsync(ephemeral: true);
        }

        [ComponentInteraction("admin-deletelist-no")]
        public async Task DeleteToDoListDeny()
        {
            //Delete response
            await Context.Interaction.GetOriginalResponseAsync().Result.DeleteAsync();
            await RespondAsync(ephemeral: true);
        }

        [SlashCommand("settodochannel", "Sets a to-do list channel, must be used with a settings menu(/admin opensettingsmenu)")]
        public async Task SetToDoListChannel(ToDoListChannelType channelType, IChannel channel = null)
        {
            if (channel == null)
            {
                await RespondAsync(ephemeral: true);
                return;
            }

            if (!await _toDoSettingsService.InstanceExists(Context.Channel.Id))
            {
                await RespondAsync(ephemeral: true, embed: new EmbedBuilder()
                    .WithTitle("Error")
                    .WithDescription("Sorry, this command can only be used with a settings menu (/admin opensettingsmenu)")
                    .Build());
            }

            ToDoList list = await GetList();

            switch (channelType)
            {
                case ToDoListChannelType.List:
                    {
                        list.ListChannelID = channel.Id;
                        break;
                    }
                case ToDoListChannelType.Command:
                    {
                        list.CommandChannelID = channel.Id;
                        break;
                    }
                case ToDoListChannelType.Forum:
                    {
                        list.ForumChannelID = channel.Id;
                        break;
                    }
            }
            await UpdateCurrentSettingsPage();
            await RespondAsync(ephemeral: true);
        }

        public async Task AddCategory(string categoryName)
        {
            ToDoList list = await GetList();
            if (list.Categories.Contains(categoryName))
            {
                await RespondAsync(ephemeral: true);
                return;
            }
            list.Categories.Add(categoryName);
            await UpdateCurrentSettingsPage();
            await RespondAsync(ephemeral: true);
        }

        public async Task RemoveCategory([Autocomplete(typeof(SettingsCategoryAutocomplete))]string categoryName)
        {
            ToDoList list = await GetList();
            if (!list.Categories.Contains(categoryName))
            {
                await RespondAsync(ephemeral: true);
                return;
            }
            list.Categories.Remove(categoryName);
            await _toDoListService.CategoryRemoved(list, categoryName);

            await UpdateCurrentSettingsPage();
            await RespondAsync(ephemeral: true);
        }

        public async Task RenameCategory([Autocomplete(typeof(SettingsCategoryAutocomplete))] string categoryName, string newName)
        {
            ToDoList list = await GetList();
            if (!list.Categories.Contains(categoryName) || list.Categories.Contains(newName))
            {
                await RespondAsync(ephemeral: true);
                return;
            }
            list.Categories.Remove(categoryName);
            list.Categories.Add(newName);
            await _toDoListService.CategoryRenamed(list, categoryName, newName);

            await UpdateCurrentSettingsPage();
            await RespondAsync(ephemeral: true);
        }

        public async Task AddStage(string stageName)
        {
            ToDoList list = await GetList();
            if (list.Stages.Contains(stageName))
            {
                await RespondAsync(ephemeral: true);
                return;
            }
            list.Stages.Add(stageName);
            await UpdateCurrentSettingsPage();
            await RespondAsync(ephemeral: true);
        }

        public async Task RemoveStage([Autocomplete(typeof(SettingsStageAutocomplete))] string stageName)
        {
            ToDoList list = await GetList();
            if (!list.Stages.Contains(stageName))
            {
                await RespondAsync(ephemeral: true);
                return;
            }
            list.Stages.Remove(stageName);
            await _toDoListService.StageRemoved(list, stageName);

            await UpdateCurrentSettingsPage();
            await RespondAsync(ephemeral: true);
        }

        public async Task RenameStage([Autocomplete(typeof(SettingsStageAutocomplete))] string stageName, string newName)
        {
            ToDoList list = await GetList();
            if (!list.Stages.Contains(stageName) || list.Stages.Contains(newName))
            {
                await RespondAsync(ephemeral: true);
                return;
            }
            list.Stages.Remove(stageName);
            list.Stages.Add(newName);
            await _toDoListService.StageRenamed(list, stageName, newName);

            await UpdateCurrentSettingsPage();
            await RespondAsync(ephemeral: true);
        }

        public async Task SetUserPermission(IUser user, ListUserPermissions permissions)
        {
            ToDoList list = await GetList();
            if (list.UserPermissions.ContainsKey(user.Id))
            {
                if (permissions is ListUserPermissions.None)
                {
                    list.UserPermissions.Remove(user.Id);
                }
                else
                {
                    list.UserPermissions[user.Id] = permissions;
                }
            }
            else if (permissions is not ListUserPermissions.None)
            {
                list.UserPermissions.Add(user.Id, permissions);
            }
        }

        public async Task SetRolePermission(IRole role, ListUserPermissions permissions)
        {
            ToDoList list = await GetList();
            if (list.RolePermissions.ContainsKey(role.Id))
            {
                if (permissions is ListUserPermissions.None)
                {
                    list.RolePermissions.Remove(role.Id);
                }
                else
                {
                    list.RolePermissions[role.Id] = permissions;
                }
            }
            else if (permissions is not ListUserPermissions.None)
            {
                list.RolePermissions.Add(role.Id, permissions);
            }
        }

        public async Task UpdateCurrentSettingsPage()
        {
            if (await _toDoSettingsService.InstanceExists(Context.Channel.Id))
            {
                MessageContext updatedPage = await _toDoSettingsService.GetCurrentSettingsPage(Context.Channel.Id);
                await _toDoSettingsService.UpdateInstance(Context.Interaction, updatedPage);
            }
        }

        public async Task<string> GetListName() => await _toDoSettingsService.GetCurrentInstanceListName(Context.Channel.Id);
        public async Task<ToDoList> GetList() => await _guildService.GetToDoList(Context.Guild.Id, await GetListName());
    }
}
