using Discord;
using Discord.Interactions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnoTasker.Modules.Preconditions.Parameters
{
    public class RequireNamingLimits : ParameterPreconditionAttribute
    {
        private static readonly char[] bannedChars = new[] { '¬', '\'' };

        public async override Task<PreconditionResult> CheckRequirementsAsync(IInteractionContext context, IParameterInfo parameterInfo, object value, IServiceProvider services)
        {
            if (value is not string stringValue) return PreconditionResult.FromError("Value is not string");

            if (stringValue.Any(x => bannedChars.Contains(x)))
                return PreconditionResult.FromError($"You cannot use the characters '{string.Join(", ", bannedChars)}' in names");
            return PreconditionResult.FromSuccess();
        }
    }
}
