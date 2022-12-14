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

namespace DiscordBot.AutocompleteHandlers
{
    public class TimeZones : AutocompleteHandler
    {
        private static readonly List<string> Timezones;
        
        static TimeZones()
        {
            Timezones = new();
            
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
            Timezones.Add(name);
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
                
                var matches = Timezones.Where(z => z.ToUpper().Contains(data.ToUpper()));
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