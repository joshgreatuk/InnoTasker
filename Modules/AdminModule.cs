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

            ToDoSettingsInstance instance = await _toDoSettingsService.GetSettingsInstance(Context.Channel.Id);

            switch (channelType)
            {
                case ToDoListChannelType.List:
                    {
                        instance.toDoChannel = channel.Id;
                        break;
                    }
                case ToDoListChannelType.Command:
                    {
                        instance.toDoCommandChannel = channel.Id;
                        break;
                    }
                case ToDoListChannelType.Forum:
                    {
                        instance.toDoForumChannel = channel.Id;
                        break;
                    }
            }
            await UpdateCurrentSettingsPage();
            await RespondAsync(ephemeral: true);
        }

        public async Task AddCategory(string categoryName)
        {
            ToDoSettingsInstance instance = await _toDoSettingsService.GetSettingsInstance(Context.Channel.Id);
            if (instance.categoryList.Contains(categoryName))
            {
                await RespondAsync(ephemeral: true);
                return;
            }
            instance.categoryList.Add(categoryName);
            await UpdateCurrentSettingsPage();
            await RespondAsync(ephemeral: true);
        }

        public async Task RemoveCategory([Autocomplete(typeof(SettingsCategoryAutocomplete))]string categoryName)
        {
            ToDoSettingsInstance instance = await _toDoSettingsService.GetSettingsInstance(Context.Channel.Id);
            if (!instance.categoryList.Contains(categoryName))
            {
                await RespondAsync(ephemeral: true);
                return;
            }
            instance.categoryList.Remove(categoryName);

            await UpdateCurrentSettingsPage();
            await RespondAsync(ephemeral: true);
        }

        public async Task RenameCategory([Autocomplete(typeof(SettingsCategoryAutocomplete))] string categoryName, string newName)
        {
            ToDoSettingsInstance instance = await _toDoSettingsService.GetSettingsInstance(Context.Channel.Id);
            if (!instance.categoryList.Contains(categoryName) || instance.categoryList.Contains(newName))
            {
                await RespondAsync(ephemeral: true);
                return;
            }
            instance.categoryList.Remove(categoryName);
            instance.categoryList.Add(newName);

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
            await RespondAsync(ephemeral: true);
        }

        public async Task AddStage(string stageName)
        {
            ToDoSettingsInstance instance = await _toDoSettingsService.GetSettingsInstance(Context.Channel.Id);
            if (instance.stageList.Contains(stageName))
            {
                await RespondAsync(ephemeral: true);
                return;
            }
            instance.stageList.Add(stageName);
            await UpdateCurrentSettingsPage();
            await RespondAsync(ephemeral: true);
        }

        public async Task RemoveStage([Autocomplete(typeof(SettingsStageAutocomplete))] string stageName)
        {
            ToDoSettingsInstance instance = await _toDoSettingsService.GetSettingsInstance(Context.Channel.Id);
            if (!instance.stageList.Contains(stageName))
            {
                await RespondAsync(ephemeral: true);
                return;
            }
            instance.stageList.Remove(stageName);

            await UpdateCurrentSettingsPage();
            await RespondAsync(ephemeral: true);
        }

        public async Task RenameStage([Autocomplete(typeof(SettingsStageAutocomplete))] string stageName, string newName)
        {
            ToDoSettingsInstance instance = await _toDoSettingsService.GetSettingsInstance(Context.Channel.Id);
            if (!instance.stageList.Contains(stageName) || instance.stageList.Contains(newName))
            {
                await RespondAsync(ephemeral: true);
                return;
            }
            instance.stageList.Remove(stageName);
            instance.stageList.Add(newName);

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
            await RespondAsync(ephemeral: true);
        }

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
        }

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
