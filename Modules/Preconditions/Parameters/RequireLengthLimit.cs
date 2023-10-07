using Discord;
using Discord.Interactions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnoTasker.Modules.Preconditions.Parameters
{
    public class RequireLengthLimit : ListParameterPrecondition
    {
        private readonly LimitType _limitType;
        public RequireLengthLimit(LimitType limit) { _limitType = limit; }

        protected async override Task<PreconditionResult> CheckRequirements(IInteractionContext context, IParameterInfo parameterInfo, object value, IServiceProvider services)
        {
            if (value is not string valueString) return FromError("Value is not string");
            Limit limit = Limits.limits[_limitType];
            if (valueString.Length > limit.limit) return FromError($"Sorry, {limit.name} can't be longer than {limit} characters (it was {valueString.Length})");
            return FromSuccess();
        }
    }
}
