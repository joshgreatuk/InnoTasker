using InnoTasker.Services.Interfaces;
using InnoTasker.Services.Interfaces.ToDo;
using InnoTasker.Data.ToDo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnoTasker.Modules.ToDo
{
    using Discord.Interactions;
    using Preconditions;

    [RequireItemChannel]
    public abstract class ItemModuleBase : ToDoModuleBase
    {
        protected ToDoItem item;

        public ItemModuleBase(IGuildService guildService, IToDoUpdateService updateService) : base(guildService, updateService) { }

        public async override Task BeforeExecuteAsync(ICommandInfo command)
        {
            await base.BeforeExecuteAsync(command);
            item = await (await _guildService.GetToDoList(Context.Guild.Id, listName)).GetToDoItemFromChannel(Context.Channel.Id);
        }
    }
}
