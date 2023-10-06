using Discord;
using Discord.Interactions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnoTasker.Modules.Preconditions.Parameters
{
    public class RequireSettingsInstance : SettingsParameterPrecondition
    {
        protected async override Task<PreconditionResult> CheckRequirements(IInteractionContext context, IParameterInfo parameterInfo, object value, IServiceProvider services)
        {
            return FromSuccess(); //If we got this far, the instance exists
        }
    }
}
