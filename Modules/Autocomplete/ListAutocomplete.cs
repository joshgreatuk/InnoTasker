using Discord;
using Discord.Interactions;
using InnoTasker.Data.ToDo;
using InnoTasker.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnoTasker.Modules.Autocomplete
{
    public class ListAutocomplete : AutocompleteHandler
    {
        public async override Task<AutocompletionResult> GenerateSuggestionsAsync(IInteractionContext context, IAutocompleteInteraction autocompleteInteraction, IParameterInfo parameter, IServiceProvider services)
        {
            IGuildService guildService = services.GetRequiredService<IGuildService>();
            List<AutocompleteResult> results = new();
            foreach (ToDoList list in guildService.GetGuildData(context.Guild.Id).Result.Lists)
            {
                results.Add(new AutocompleteResult(list.Name, list.Name));
            }
            return AutocompletionResult.FromSuccess(results.Take(25));
        }
    }
}
