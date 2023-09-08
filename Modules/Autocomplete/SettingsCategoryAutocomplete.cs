﻿using Discord;
using Discord.Interactions;
using InnoTasker.Data.ToDo;
using InnoTasker.Services.Interfaces;
using InnoTasker.Services.Interfaces.ToDo;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnoTasker.Modules.Autocomplete
{
    public class SettingsCategoryAutocomplete : AutocompleteHandler
    {
        public async override Task<AutocompletionResult> GenerateSuggestionsAsync(IInteractionContext context, IAutocompleteInteraction autocompleteInteraction, IParameterInfo parameter, IServiceProvider services)
        {
            IToDoSettingsService settingsService = services.GetRequiredService<IToDoSettingsService>();
            if (!await settingsService.InstanceExists(context.Channel.Id))
            {
                return AutocompletionResult.FromError(new Exception($"No settings menu instance found"));
            }

            IGuildService guildService = services.GetRequiredService<IGuildService>();
            string listName = await settingsService.GetCurrentInstanceListName(context.Channel.Id);

            ToDoList list = await guildService.GetToDoList(context.Guild.Id, listName);
            return AutocompletionResult.FromSuccess(list.Categories.Select(x => new AutocompleteResult(x, x)).Take(25));
        }
    }
}
