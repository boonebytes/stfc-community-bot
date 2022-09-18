using Discord;
using Discord.Interactions;
using DiscordBot.Domain.Entities.Zones;

namespace DiscordBot.AutocompleteHandlers
{
    public class ZoneNames : AutocompleteHandler
    {
        private static readonly List<string> Zones = new();

        private static void AddZone(string name)
        {
            Zones.Add(name);
        }

        public override async Task<AutocompletionResult> GenerateSuggestionsAsync(
            IInteractionContext context,
            IAutocompleteInteraction autocompleteInteraction,
            IParameterInfo parameter,
            IServiceProvider services)
        {
            var logger = services.GetRequiredService<ILogger<ZoneNames>>();
            try
            {
                if (!Zones.Any())
                {
                    using var thisServiceScope = services.CreateScope();
                    var zoneRepository = thisServiceScope.ServiceProvider.GetRequiredService<IZoneRepository>();
                    var allZones = await zoneRepository.GetLookupListAsync();
                    foreach (var z in allZones)
                    {
                        AddZone(z.Name);
                    }
                }

                var data = autocompleteInteraction.Data.Current.Value as string;
                if (string.IsNullOrEmpty(data))
                    return AutocompletionResult.FromSuccess();
                
                var matches = Zones.Where(z => z.ToUpper().StartsWith(data.ToUpper()));
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