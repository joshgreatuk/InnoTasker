using Discord;
using Discord.Interactions;
using InnoTasker.Data.ToDo;
using InnoTasker.Modules.Preconditions.Parameters;
using InnoTasker.Services.Interfaces.ToDo;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnoTasker.Modules.Preconditions
{
    public class RequireSettingsInstance : PreconditionAttribute
    {
        public async override Task<PreconditionResult> CheckRequirementsAsync(IInteractionContext context, ICommandInfo commandInfo, IServiceProvider services)
        {
            IToDoSettingsService settingsService = services.GetRequiredService<IToDoSettingsService>();
            if (!await settingsService.InstanceExists(context.Channel.Id)) return PreconditionResult.FromError("Sorry, this must be done with a setting instance open (/admin opensettingsmenu)");
            return PreconditionResult.FromSuccess();
        }
    }
}
