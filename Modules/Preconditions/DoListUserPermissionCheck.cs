using Discord;
using Discord.Interactions;
using InnoTasker.Data.ToDo;
using InnoTasker.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace InnoTasker.Modules.Preconditions
{
    public class DoListUserPermissionCheck : PreconditionAttribute
    {
        public ListUserPermissions permissionRequired;

        public DoListUserPermissionCheck(ListUserPermissions permissionRequired)
        {
            this.permissionRequired = permissionRequired;
        }

        public async override Task<PreconditionResult> CheckRequirementsAsync(IInteractionContext context, ICommandInfo command, IServiceProvider services)
        {
            IGuildService guildService = services.GetRequiredService<IGuildService>();
            ToDoList? targetList = await guildService.GetToDoListFromChannel(context.Guild.Id, context.Channel.Id);
            if (targetList == null) return PreconditionResult.FromError("This command must be done in a to-do list channel");

            bool hasPermittedRole = false;
            IGuildUser user = await context.Guild.GetUserAsync(context.User.Id);
            foreach (ulong roleId in user.RoleIds.Where(x => targetList.RolePermissions.ContainsKey(x)))
            {
                if (targetList.RolePermissions[roleId] >= permissionRequired)
                {
                    hasPermittedRole = true;
                    break;
                }
            }

            if (!hasPermittedRole && (!targetList.UserPermissions.TryGetValue(context.User.Id, out ListUserPermissions permissions) || permissions < permissionRequired))
            {
                return PreconditionResult.FromError($"You need {permissionRequired} permissions to run this command");
            }

            return PreconditionResult.FromSuccess();
        }
    }
}
