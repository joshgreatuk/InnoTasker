using Discord;
using Discord.Interactions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnoTasker.Modules.Preconditions.Parameters
{
    public class RequireStageExists : ListParameterPrecondition
    {
        protected async override Task<PreconditionResult> CheckRequirements(IInteractionContext context, IParameterInfo parameterInfo, object value, IServiceProvider services)
        {
            if (value is not string stage) return FromError("Value is not string");

            if (!list.Stages.Contains(stage)) return FromError("Stage doesn't exist");
            return FromSuccess();
        }
    }
}
