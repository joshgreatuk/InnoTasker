using Discord;
using Discord.Interactions;
using InnoTasker.Services.Interfaces;
using InnoTasker.Services.Interfaces.ToDo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnoTasker.Modules.ToDo
{
    [Group("to-do-item-admin", "Admin commands for a task inside it's forum post")]
    public class ToDoItemAdminModule : ItemModuleBase
    {
        public ToDoItemAdminModule(IGuildService guildService, IToDoUpdateService updateService) : base(guildService, updateService) { }

        public async Task AddUser(IGuildUser user)
        {
            await _updateService.TaskAddUser(Context.Guild.Id, listName, item.ID, user);
        }

        public async Task RemoveUser(IGuildUser user)
        {
            await _updateService.TaskRemoveUser(Context.Guild.Id, listName, item.ID, user);
        }
    }
}
