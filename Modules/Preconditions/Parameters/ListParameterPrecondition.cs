using Discord;
using Discord.Interactions;
using InnoTasker.Data;
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
    public abstract class ListParameterPrecondition : InnoParamterPrecondition
    {
        protected IGuildService guildService;
        protected ToDoList list;

        public async override Task<PreconditionResult> CheckRequirementsAsync(IInteractionContext context, IParameterInfo parameterInfo, object value, IServiceProvider services)
        {
            guildService = services.GetRequiredService<IGuildService>();
            list = await guildService.GetToDoListFromChannel(context.Channel.Id);

            if (list == null)
            {
                return FromError("Not a command channel");
            }

            return await CheckRequirements(context, parameterInfo, value, services);
        }
    }
}
