using Discord;
using Discord.Interactions;
using InnoTasker.Modules.Autocomplete;
using InnoTasker.Services.Interfaces.ToDo;
using InnoTasker.Data.ToDo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace InnoTasker.Modules
{
    using global::InnoTasker.Services.Interfaces;
    using Preconditions;

    [Group("to-do-admin", "Admin commands for a to-do list")]
    [DoListUserPermissionCheck(ListUserPermissions.Admin)]
    public class ToDoAdminModule : ToDoModuleBase
    {
        public ToDoAdminModule(IGuildService guildService, IToDoUpdateService updateService) : base(guildService, updateService) { }

        [SlashCommand("generate-supportid", "Generates a support ID")]
        public async Task GenerateSupportID()
        {
            SupportInfo info = new SupportInfo(Context.Guild.Id,
                Context.Channel.Id,
                Context.User.Id,
                await _updateService.GetListNameFromChannel(Context.Guild.Id, Context.Channel.Id));
            string infoID = Support.GenerateSupportID(info);

            success = false;
            await FollowupAsync(embed: new EmbedBuilder()
                .WithTitle("Your support ID")
                .WithDescription($"**{infoID}**\n\nWe recommend that you do not share this ID with anyone but the InnoTasker support team")
                .Build(), 
                ephemeral: true);
        }
    }
}
