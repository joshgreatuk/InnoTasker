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
        private readonly Limit _limit;
        public RequireLengthLimit(Limit limit) { _limit = limit; }

        protected async override Task<PreconditionResult> CheckRequirements(IInteractionContext context, IParameterInfo parameterInfo, object value, IServiceProvider services)
        {
            if (value is not string valueString) return FromError("Value is not string");

            if (valueString.Length > _limit.limit) return FromError($"Sorry, {_limit.name} can't be longer than {_limit.limit} characters (it was {valueString.Length})");
            return FromSuccess();
        }
    }
}
