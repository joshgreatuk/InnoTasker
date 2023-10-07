using Discord.Interactions;
using InnoTasker.Services.Interfaces.ToDo;
using InnoTasker.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InnoTasker.Modules.Preconditions;

namespace InnoTasker.Modules.ToDo
{
    [DoListCommandChannelCheck()]
    public class ToDoModuleBase : InnoModuleBase
    {
        protected readonly IGuildService _guildService;
        protected readonly IToDoUpdateService _updateService;

        protected string listName;

        public ToDoModuleBase(IGuildService guildService, IToDoUpdateService updateService)
        {
            _guildService = guildService;
            _updateService = updateService;
        }

        public async override Task BeforeExecuteAsync(ICommandInfo command)
        {
            await base.BeforeExecuteAsync(command);
            listName = await _updateService.GetListNameFromChannel(Context.Guild.Id, Context.Channel.Id);
        }
    }
}
