using Discord;
using Discord.Interactions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnoTasker.Modules.Preconditions.Parameters
{
    public class RequireCategoryExists : ListParameterPrecondition
    {
        protected async override Task<PreconditionResult> CheckRequirements(IInteractionContext context, IParameterInfo parameterInfo, object value, IServiceProvider services)
        {
            if (value is not string category) return FromError("Value is not string");

            if (!list.Categories.Contains(category)) return FromError("Category doesn't exist");
            return FromSuccess();
        }
    }
}
