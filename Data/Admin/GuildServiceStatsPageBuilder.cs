using Discord;
using Discord.WebSocket;
using InnoTasker.Data.Databases;
using InnoTasker.Data.ToDo;
using InnoTasker.Modules.Settings;
using InnoTasker.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnoTasker.Data.Admin
{
    /* Guild stats must include:
     * - Number of active guilds
     * - Total to-do lists
     * - 
     */
    public class GuildServiceStatsPageBuilder : ISettingsPageBuilder
    {
        private readonly GuildDatabase _guildDatabase;
        private readonly IGuildService _guildService;

        public GuildServiceStatsPageBuilder(GuildDatabase guildDatabase, IGuildService guildService)
        {
            _guildDatabase = guildDatabase;
            _guildService = guildService;
        }

        public async Task<MessageContext> BuildPage(ToDoSettingsInstance settings)
        {
            List<string> fields = new()
            {
                $"**Active Guilds:** {_guildDatabase.Count}",
                $"**To-Do List Count:** {_guildDatabase.Sum(x => x.Value.Lists.Count)}"
            };

            Embed embed = new EmbedBuilder()
                .WithTitle("Guild Service Statistics")
                .WithDescription(String.Join("\n", fields))
                .WithCurrentTimestamp()
                .Build();
            return new MessageContext(embed, new ComponentBuilder());
        }

        public async Task<MessageContext?> HandleInteraction(ToDoSettingsInstance settings, SocketInteraction interaction)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> CanProceed(ToDoSettingsInstance settings) => true;
    }
}
