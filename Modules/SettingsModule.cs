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
    public class SettingsModule : InteractionModuleBase<SocketInteractionContext>
    {
        private readonly IToDoSettingsService _settingsService;

        public SettingsModule(IToDoSettingsService settingsService)
        {
            _settingsService = settingsService;
        }

        public async Task NextPageInteraction()
        {

        }

        public async Task ClosePage()
        {

        }

        public async Task LastPageInteraction()
        {

        }
    }
}
