/*
Copyright 2022 Boonebytes

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/

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