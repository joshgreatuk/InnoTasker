using Discord.Interactions;
using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InnoTasker.Data.ToDo;
using InnoTasker.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace InnoTasker.Modules.Autocomplete
{
    public class StageAutocomplete : AutocompleteHandler
    {
        public async override Task<AutocompletionResult> GenerateSuggestionsAsync(IInteractionContext context, IAutocompleteInteraction autocompleteInteraction, IParameterInfo parameter, IServiceProvider services)
        {
            IGuildService guildService = services.GetRequiredService<IGuildService>();
            ToDoList list = await guildService.GetToDoListFromChannel(context.Channel.Id);
            List<AutocompleteResult> suggestions = list.Stages.Select(x => new AutocompleteResult(x, x)).ToList();
            return AutocompletionResult.FromSuccess(suggestions.Take(25));
        }
    }
}
