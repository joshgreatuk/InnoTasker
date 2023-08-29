using Discord;
using Discord.Interactions;
using InnoTasker.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnoTasker.Modules.Preconditions
{
    public class DoListCommandChannelCheck : PreconditionAttribute
    { 
        public async override Task<PreconditionResult> CheckRequirementsAsync(IInteractionContext context, ICommandInfo commandInfo, IServiceProvider services)
        {
            IGuildService guildService = services.GetRequiredService<IGuildService>();
            if (guildService.GetToDoListFromChannel(context.Guild.Id, context.Channel.Id) != null)
            {
                return PreconditionResult.FromSuccess();
            }
            return PreconditionResult.FromError("This command must be done in a to-do list channel");
        }
    }
}
