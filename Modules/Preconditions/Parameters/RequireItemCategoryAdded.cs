using Discord;
using Discord.Interactions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnoTasker.Modules.Preconditions.Parameters
{
    public class RequireItemCategoryAdded : ItemParameterPrecondition
    {
        private bool _invert;

        public RequireItemCategoryAdded(bool invert=false) 
        {
            _invert = invert;
        }

        protected async override Task<PreconditionResult> CheckRequirements(IInteractionContext context, IParameterInfo parameterInfo, object value, IServiceProvider services)
        {
            if (value is not string category) return FromError("Value is not string");

            if (item.Categories.Contains(category))
            {
                if (_invert) return FromError("Category is already added");
                return FromSuccess();
            }

            if (_invert) return FromSuccess();
            return FromError("Category is not applied");
        }
    }
}
