using Discord;
using Discord.Interactions;
using InnoTasker.Data;
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
            MessageContext message = await _settingsService.GetNextSettingsPage(Context.Channel.Id);
            await _settingsService.UpdateInstance(Context.Interaction, message);
            await RespondAsync(ephemeral: true);
        }

        [ComponentInteraction("settings-close")]
        public async Task ClosePage()
        {
            await _settingsService.CloseInstance(Context.Channel.Id);
            await RespondAsync(ephemeral: true);
        }

        [ComponentInteraction("settings-last")]
        public async Task LastPageInteraction()
        {
            MessageContext message = await _settingsService.GetLastSettingsPage(Context.Channel.Id);
            await _settingsService.UpdateInstance(Context.Interaction, message);
            await RespondAsync(ephemeral:true);
        }

        [ComponentInteraction("settings-comp-*")]
        public async Task HandleSettingsInteraction()
        {
            await _settingsService.HandleInteraction(Context.Interaction);
            await RespondAsync(ephemeral: true);
        }
    }
}
