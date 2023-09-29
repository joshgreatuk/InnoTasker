using Discord;
using Discord.Interactions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnoTasker.Modules.Preconditions
{
    public class RequireBotAdmin : PreconditionAttribute
    {
        private static readonly ulong[] s_BotAdmins = { 201640887325949953 };

        public async override Task<PreconditionResult> CheckRequirementsAsync(IInteractionContext context, ICommandInfo commandInfo, IServiceProvider services)
        {
            if (s_BotAdmins.Contains(context.User.Id)) return PreconditionResult.FromSuccess();

            return PreconditionResult.FromError("You aren't a bot admin");
        }
    }
}
