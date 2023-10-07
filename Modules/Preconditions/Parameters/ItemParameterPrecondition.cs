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

namespace InnoTasker.Modules.Preconditions.Parameters
{
    public abstract class ItemParameterPrecondition : ListParameterPrecondition
    {
        protected ToDoItem item;        

        public async override Task<PreconditionResult> CheckRequirementsAsync(IInteractionContext context, IParameterInfo parameterInfo, object value, IServiceProvider services)
        {
            guildService = services.GetRequiredService<IGuildService>();
            list = await guildService.GetToDoListFromChannel(context.Channel.Id);
            if (list == null) return FromError("Not a command channel");

            item = await list.GetToDoItemFromChannel(context.Channel.Id);
            if (item == null) return FromError("This must be done in a task's forum post");

            return await CheckRequirements(context, parameterInfo, value, services);
        }
    }
}
