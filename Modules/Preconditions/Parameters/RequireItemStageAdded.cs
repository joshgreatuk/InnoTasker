using Discord.Interactions;
using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnoTasker.Modules.Preconditions.Parameters
{
    public class RequireItemStageAdded : ItemParameterPrecondition
    {
        private bool _invert;

        public RequireItemStageAdded(bool invert = false)
        {
            _invert = invert;
        }

        protected async override Task<PreconditionResult> CheckRequirements(IInteractionContext context, IParameterInfo parameterInfo, object value, IServiceProvider services)
        {
            if (value is not string stage) return FromError("Value is not string");

            if (item.Stages.Contains(stage))
            {
                if (_invert) return FromError("Stage is already added");
                return FromSuccess();
            }

            if (_invert) return FromSuccess();
            return FromError("Stage is not applied");
        }
    }
}
