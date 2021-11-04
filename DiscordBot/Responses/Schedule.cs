using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using DiscordBot.Domain.Entities.Zones;

namespace DiscordBot.Responses
{
    public class Schedule
    {
        private readonly IZoneRepository _zoneRepository;

        public Schedule(IZoneRepository zoneRepository)
        {
            _zoneRepository = zoneRepository;
        }

        protected void AddDefendsToEmbed(List<Zone> zones, ref EmbedBuilder embedMsg, bool shortVersion = false)
        {
            if (zones.Count() == 0)
            {
                var thisField = new EmbedFieldBuilder
                {
                    Name = "None Scheduled",
                    Value = "There are no defends scheduled."
                };
                embedMsg.AddField(thisField);
            }
            else
            {
                int currentLine = 0;
                foreach (Zone zone in zones)
                {
                    if (currentLine > 0)
                    {
                        //embedMsg.AddField("\u200B", "\u200B", true);
                        //embedMsg.AddField("\b", "\b", true);
                        //embedMsg.AddField("\u200b", "\u200b", true);
                    }
                    currentLine++;
                    if (shortVersion)
                    {
                        embedMsg.Description += zone.GetDiscordEmbedValue(true) + "\n\u200b";
                    }
                    else
                    {
                        var thisField = new EmbedFieldBuilder
                        {
                            Name = zone.GetDiscordEmbedName(),
                            Value = zone.GetDiscordEmbedValue(false) + "\n\u200b"
                        };
                        embedMsg.AddField(thisField);
                    }
                }
            }
        }

        public EmbedBuilder GetForDate(DateTime date, long? allianceId = null, bool shortVersion = false)
        {
            var embedMsg = new EmbedBuilder
            {
                Title = "Defend Schedule for " + date.ToString("dddd, MMM d")
                //Description = ""
            };

            var fromDate = date.ToUniversalTime();
            if (fromDate.Hour < 4)
            {
                fromDate = fromDate.AddDays(-1);
            }
            fromDate = fromDate.AddHours(-date.ToUniversalTime().Hour + 3);

            var todayDefends = _zoneRepository.GetNext24Hours(fromDate, allianceId).OrderBy(z => z.NextDefend).ToList();

            AddDefendsToEmbed(todayDefends, ref embedMsg, shortVersion);

            return embedMsg;
        }

        public EmbedBuilder GetNext(long? allianceId = null)
        {
            var embedMsg = new EmbedBuilder
            {
                Title = "Next Defense"
                //Description = ""
            };
            var nextDefend = _zoneRepository.GetNextDefend(allianceId);
            if (nextDefend == null)
            {
                embedMsg.Description = "No defends were found.";
                return embedMsg;
            }
            else
            {
                var thisField = new EmbedFieldBuilder
                {
                    Name = nextDefend.GetDiscordEmbedName(),
                    Value = nextDefend.GetDiscordEmbedValue() + "\n\u200b"
                };
                embedMsg.AddField(thisField);
                return embedMsg;
            }
        }

        public async Task<EmbedBuilder> GetAll()
        {
            var embedMsg = new EmbedBuilder
            {
                Title = "Full Defend Schedule"
                //Description = ""
            };

            var allDefends = (await _zoneRepository.GetAllAsync()).OrderBy(z => z.NextDefend).ToList();

            AddDefendsToEmbed(allDefends, ref embedMsg);

            return embedMsg;
        }
    }
}
