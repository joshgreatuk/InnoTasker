using Discord;
using Discord.Interactions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnoTasker.Modules.Preconditions.Parameters
{
    public class RequireCategorySpace : SettingsParameterPrecondition
    {
        protected async override Task<PreconditionResult> CheckRequirements(IInteractionContext context, IParameterInfo parameterInfo, object value, IServiceProvider services)
        {
            if (_instance.categoryList.Count >= 14) return FromError("Sorry, you can't have more than 14 categories");
            return FromSuccess();
        }
    }
}
