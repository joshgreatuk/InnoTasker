using Discord;
using Discord.Interactions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnoTasker.Modules.Preconditions.Parameters
{
    public class RequireItemExists : ListParameterPrecondition
    {
        protected async override Task<PreconditionResult> CheckRequirements(IInteractionContext context, IParameterInfo parameterInfo, object value, IServiceProvider services)
        {
            if (value is not int taskID) return FromError("Value is not int");

            if (list.Items.Any(x => x.ID != taskID)) return FromError("TaskID doesnt exist");

            return FromSuccess();
        }
    }
}
