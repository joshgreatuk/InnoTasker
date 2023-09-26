using Discord;
using Discord.Interactions;
using InnoTasker.Data;
using InnoTasker.Data.ToDo;
using InnoTasker.Services.Interfaces.ToDo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnoTasker.Modules
{
    //To handle generic interactions from the settings pages (next, previous, close)
    public class SettingsPagesModule : InnoModuleBase
    {
        private readonly IToDoSettingsService _settingsService;
        private readonly IToDoListService _toDoService;

        public SettingsPagesModule(IToDoSettingsService settingsService, IToDoListService toDoService)
        {
            _settingsService = settingsService;
            _toDoService = toDoService;
        }

        [ComponentInteraction("settings-next")]
        public async Task NextPageInteraction()
        {
            MessageContext message = await _settingsService.GetNextSettingsPage(Context.Channel.Id);
            await _settingsService.UpdateInstance(Context.Interaction, message);
            success = false;
        }

        [ComponentInteraction("settings-close")]
        public async Task ClosePage()
        {
            //Instance will already be closed if there was an error
            if (await _settingsService.SaveInstance(Context.Channel.Id))
            {
                ToDoSettingsInstance instance = await _settingsService.GetSettingsInstance(Context.Channel.Id);
                if (instance.context is ToDoSettingsContext.New)
                {
                    
                }
                await _toDoService.UpdateToDoList(instance.guildID, instance.toDoListName, instance.categoriesRenamed, instance.stagesRenamed);
                await _settingsService.CloseInstance(Context.Channel.Id);
            }
            success = false;
        }

        [ComponentInteraction("settings-list-close")]
        public async Task CloseListPage()
        {
            await _settingsService.CloseInstance(Context.Channel.Id);
            success = false;
        }

        [ComponentInteraction("settings-last")]
        public async Task LastPageInteraction()
        {
            MessageContext message = await _settingsService.GetLastSettingsPage(Context.Channel.Id);
            await _settingsService.UpdateInstance(Context.Interaction, message);
            success = false;
        }

        [ComponentInteraction("settings-comp-*")]
        public async Task HandleSettingsInteraction()
        {
            await _settingsService.HandleInteraction(Context.Interaction);
            success = false;
        }
    }
}
