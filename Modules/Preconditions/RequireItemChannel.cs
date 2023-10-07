using Discord;
using Discord.Interactions;
using InnoTasker.Data.ToDo;
using InnoTasker.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnoTasker.Modules.Preconditions
{
    public class RequireItemChannel : PreconditionAttribute
    {
        public async override Task<PreconditionResult> CheckRequirementsAsync(IInteractionContext context, ICommandInfo commandInfo, IServiceProvider services)
        {
            IGuildService guildService = services.GetRequiredService<IGuildService>();
            ToDoList list = await guildService.GetToDoListFromChannel(context.Guild.Id, context.Channel.Id);
            if (list == null) return PreconditionResult.FromError("This command must be done in a to-do list channel");
            ToDoItem item = await list.GetToDoItemFromChannel(context.Channel.Id);
            if (item == null) return PreconditionResult.FromError("Sorry, this command has to be done in a task forum post");
            return PreconditionResult.FromSuccess();
        }
    }
}
