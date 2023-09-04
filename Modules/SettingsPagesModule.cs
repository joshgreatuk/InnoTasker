using Discord;
using Discord.Interactions;
using InnoTasker.Services.Interfaces.ToDo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnoTasker.Modules
{
    //To handle generic interactions from the settings pages (next, previous, close)
    public class SettingsPagesModule : InteractionModuleBase<SocketInteractionContext>
    {
        private readonly IToDoSettingsService _settingsService;

        public SettingsPagesModule(IToDoSettingsService settingsService)
        {
            _settingsService = settingsService;
        }

        [ComponentInteraction("settings-next")]
        public async Task NextPageInteraction()
        {
            
        }

        [ComponentInteraction("settings-close")]
        public async Task ClosePage()
        {
            
        }

        [ComponentInteraction("settings-last")]
        public async Task LastPageInteraction()
        {

        }
    }
}
