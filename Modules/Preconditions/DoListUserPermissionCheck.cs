using Discord.Commands;
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

        public async override Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, CommandInfo command, IServiceProvider services)
        {
            IGuildService guildService = services.GetRequiredService<IGuildService>();
            ToDoList? targetList = guildService.GetToDoListFromChannel(context.Guild.Id, context.Channel.Id);
            if (targetList == null) return PreconditionResult.FromError("This command must be done in a to-do list channel");

            if (!targetList.UserPermissions.TryGetValue(context.User.Id, out ListUserPermissions permissions) || !permissions.HasFlag(permissionRequired))
            {
                return PreconditionResult.FromError($"You must need {permissionRequired} permissions to run this command");
            }

            return PreconditionResult.FromSuccess();
        }
    }
}
