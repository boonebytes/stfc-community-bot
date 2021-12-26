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
    public class ZoneNames : AutocompleteHandler
    {
        protected static List<string> Zones;

        static ZoneNames()
        {
            Zones = new();
            AddZone("Abilakk");
            AddZone("Adia");
            AddZone("Anzat");
            AddZone("Aonad");
            AddZone("Asiti");
            AddZone("Avansa");
            AddZone("Aylus");
            AddZone("Barasa");
            AddZone("Beku");
            AddZone("Ber'Tho");
            AddZone("Bimasa");
            AddZone("Bolari");
            AddZone("Brellan");
            AddZone("Brijac");
            AddZone("Burran");
            AddZone("Comst");
            AddZone("Corva");
            AddZone("Crios");
            AddZone("Duportas");
            AddZone("Eldur");
            AddZone("Ezla");
            AddZone("Framtid");
            AddZone("Gelida");
            AddZone("Helvi");
            AddZone("Hoobishan");
            AddZone("Hrojost");
            AddZone("Innlasn");
            AddZone("Klefaski");
            AddZone("Kolava");
            AddZone("Lenara");
            AddZone("Mak'ala");
            AddZone("Nujord");
            AddZone("Nyrheimur");
            AddZone("Otima");
            AddZone("Parturi");
            AddZone("Perim");
            AddZone("Qeyma");
            AddZone("Roshar");
            AddZone("Ruhe");
            AddZone("Saldeti");
            AddZone("Stilhe");
            AddZone("Tazolka");
            AddZone("Tefkari");
            AddZone("Temeri");
            AddZone("Tezera");
            AddZone("Thaylen");
            AddZone("Tholus");
            AddZone("Thosz");
            AddZone("Tigan");
            AddZone("Triss");
            AddZone("Vantar");
            AddZone("Vemira");
            AddZone("Zamaro");
            AddZone("Zhian");
        }

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
                //var zoneRepository = services.GetRequiredService<IZoneRepository>();
                
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