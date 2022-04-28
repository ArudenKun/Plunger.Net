// using System;
// using System.Collections.Generic;
// using System.Linq;
// using System.Threading.Tasks;
// using Discord;
// using Discord.Interactions;

// namespace Plunger.Attributes;

// public class AutoComplete : AutocompleteHandler
// {
//     public override async Task<AutocompletionResult> GenerateSuggestionsAsync(IInteractionContext context, IAutocompleteInteraction autocompleteInteraction, IParameterInfo parameter, IServiceProvider services)
//     {
//         var subject = autocompleteInteraction.Data.Current.Value as string; // what the user managed to type into the textbox so far
//         string[] files = { "test1", "test2" };
//         IEnumerable<string> maps = files;

//         var autocompleteResults = files.Select(s => new AutocompleteResult
//         {
//             Name = s, // here's what will appear in the suggestions list
//             Value = s // here's what will actually go into the slashcommand argument on tapping the suggestion
//         });

//         return AutocompletionResult.FromSuccess(autocompleteResults);
//     }
// }
