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
    public class TimeZones : AutocompleteHandler
    {
        private static List<string> _timezones = null;
        
        static TimeZones()
        {
            _timezones = new();
            
            TimeZoneInfo.FindSystemTimeZoneById("America/New_York");
            var tzs = TimeZoneInfo.GetSystemTimeZones()
                .OrderBy(z => z.Id);
            foreach (var tz in tzs)
            {
                AddTimeZone(tz.Id);
            }
        }

        private static void AddTimeZone(string name)
        {
            _timezones.Add(name);
        }

        public override async Task<AutocompletionResult> GenerateSuggestionsAsync(
            IInteractionContext context,
            IAutocompleteInteraction autocompleteInteraction,
            IParameterInfo parameter,
            IServiceProvider services)
        {
            var logger = services.GetRequiredService<ILogger<TimeZones>>();
            try
            {
                var data = autocompleteInteraction.Data.Current.Value as string;
                if (string.IsNullOrEmpty(data))
                    return AutocompletionResult.FromSuccess();
                
                var matches = _timezones.Where(z => z.ToUpper().Contains(data.ToUpper()));
                return AutocompletionResult.FromSuccess(
                            matches.Select(m => new AutocompleteResult(m, m))
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