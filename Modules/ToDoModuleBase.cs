﻿using Discord.Interactions;
using InnoTasker.Services.Interfaces.ToDo;
using InnoTasker.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InnoTasker.Modules.Preconditions;

namespace InnoTasker.Modules
{
    [DoListCommandChannelCheck()]
    public class ToDoModuleBase : InteractionModuleBase<SocketInteractionContext>
    {
        protected readonly IGuildService _guildService;
        protected readonly IToDoUpdateService _updateService;

        protected string listName;
        protected bool success = true;

        public ToDoModuleBase(IGuildService guildService, IToDoUpdateService updateService)
        {
            _guildService = guildService;
            _updateService = updateService;
        }

        public override async Task BeforeExecuteAsync(ICommandInfo command)
        {
            await DeferAsync();
            listName = await _updateService.GetListNameFromChannel(Context.Guild.Id, Context.Channel.Id);
        }

        public override async Task AfterExecuteAsync(ICommandInfo command)
        {
            if (success)
            {
                await FollowupAsync("Done!");
                await DeleteOriginalResponseAsync();
            }
        }

        public async Task<bool> DoesTaskExist(int taskID)
        {
            bool result = (await _guildService.GetToDoList(Context.Guild.Id, listName)).Items.Exists(x => x.ID == taskID);
            if (!result)
            {
                await FollowupAsync($"Task '#{taskID}' doesn't exist!");
                success = false;
            }
            return result;
        }

        public async Task<bool> DoesCategoryExist(string category)
        {
            bool result = (await _guildService.GetToDoList(Context.Guild.Id, listName)).Categories.Exists(x => x == listName);
            if (!result)
            {
                await FollowupAsync($"Category '{category}' doesn't exist!");
                success = false;
            }
            return result;
        }

        public async Task<bool> DoesStageExist(string stage)
        {
            bool result = (await _guildService.GetToDoList(Context.Guild.Id, listName)).Stages.Exists(x => x == stage);
            if (!result)
            {
                await FollowupAsync($"Stage '{stage}' doesn't exist!");
                success = false;
            }
            return result;
        }
    }
}