using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Interactions;
using DiscordBot.Domain.Entities.Zones;
using DiscordBot.Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace DiscordBot.AutocompleteHandlers
{
    public class VariableNames : AutocompleteHandler
    {
        public static class VariableNameKeys
        {
            public const string BroadcastLeadTime = "BroadcastLeadTime";
        }
        
        protected static Dictionary<string, string> _variables = new();

        static VariableNames()
        {
            AddVariable(VariableNameKeys.BroadcastLeadTime,
                "Time (in minutes) ahead of territory events in which a reminder will be sent to everyone");
        }

        private static void AddVariable(string name, string description)
        {
            _variables.Add(name, description);
        }

        public override async Task<AutocompletionResult> GenerateSuggestionsAsync(
            IInteractionContext context,
            IAutocompleteInteraction autocompleteInteraction,
            IParameterInfo parameter,
            IServiceProvider services)
        {
            var logger = services.GetRequiredService<ILogger<VariableNames>>();
            try
            {
                //var zoneRepository = services.GetRequiredService<IZoneRepository>();
                
                var data = autocompleteInteraction.Data.Current.Value as string;
                if (string.IsNullOrEmpty(data))
                    return AutocompletionResult.FromSuccess();
                
                var matches = _variables
                    .Where(kbp => kbp.Key.ToUpper().Contains(data.ToUpper()));
                return AutocompletionResult.FromSuccess(
                            matches.Select(m => new AutocompleteResult(m.Key, m.Value))
                        );
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unexpected error");
                return AutocompletionResult.FromError(new Exception("An unexpected error has occured"));
            }
        }
    }
}