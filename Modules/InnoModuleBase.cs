using Discord.Interactions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnoTasker.Modules
{
    public class InnoModuleBase : InteractionModuleBase<SocketInteractionContext>
    {
        protected bool success = true;

        public override async Task BeforeExecuteAsync(ICommandInfo command)
        {
            await DeferAsync();
        }

        public override async Task AfterExecuteAsync(ICommandInfo command)
        {
            if (success)
            {
                await FollowupAsync("Done!");
                await DeleteOriginalResponseAsync();
            }
        }
    }
}
