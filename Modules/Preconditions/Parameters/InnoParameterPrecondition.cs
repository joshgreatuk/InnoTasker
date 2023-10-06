using Discord;
using Discord.Interactions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnoTasker.Modules.Preconditions.Parameters
{
    public abstract class InnoParamterPrecondition : ParameterPreconditionAttribute
    {
        protected abstract Task<PreconditionResult> CheckRequirements(IInteractionContext context, IParameterInfo parameterInfo, object value, IServiceProvider services);

        protected PreconditionResult FromSuccess() => PreconditionResult.FromSuccess();
        protected PreconditionResult FromError(string reason) => PreconditionResult.FromError(reason);
    }
}
