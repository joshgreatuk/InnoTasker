using Discord;
using Discord.Interactions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnoTasker.Modules
{
    [Group("innotasker", "Innotasker bot commands")]
    public class InnoTaskerModule : InnoModuleBase
    {
        [SlashCommand("info", "Shows bot info")]
        public async Task Info()
        {
            List<string> fields = new() 
            {
                $"**Version:** {Environment.Version}",
                $"**Developed by:** {MentionUtils.MentionUser(201640887325949953)}", 
                $"",
                $"**Thanks for using my bot!"
            };

            success = false;
            EmbedBuilder embed = new EmbedBuilder()
                .WithTitle("InnoTasker")
                .WithDescription(string.Join("\n", fields))
                .WithCurrentTimestamp()
                .WithColor(Color.DarkBlue);
            await FollowupAsync(embed: embed.Build());
        }
    }
}
