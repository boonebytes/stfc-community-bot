using Discord;
using Discord.Interactions;

namespace DiscordBot.AutocompleteHandlers
{
    public class VariableNames : AutocompleteHandler
    {
        public static class VariableNameKeys
        {
            public const string BroadcastLeadTime = "BroadcastLeadTime";
            public const string AlliedBroadcastRole = "AlliedBroadcastRole";
        }
        
        protected static readonly Dictionary<string, string> Variables = new();

        static VariableNames()
        {
            AddVariable(VariableNameKeys.BroadcastLeadTime,
                "Time (in minutes) ahead of territory events in which a reminder will be sent to everyone");
            AddVariable(VariableNameKeys.AlliedBroadcastRole,
                "Discord role to ping for allied defends");
        }

        private static void AddVariable(string name, string description)
        {
            Variables.Add(name, description);
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
                    return AutocompletionResult.FromSuccess(
                        Variables.Select(v => new AutocompleteResult(v.Key, v.Key))
                        );
                
                var matches = Variables
                    .Where(kbp => kbp.Key.ToUpper().Contains(data.ToUpper()));
                return AutocompletionResult.FromSuccess(
                            matches.Select(m => new AutocompleteResult(m.Key, m.Key))
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