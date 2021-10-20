using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;

namespace DiscordBot.Responses
{
    public class Schedule
    {
        private readonly Managers.DefendTimes _defendTimes;

        public Schedule(Managers.DefendTimes defendTimes)
        {
            _defendTimes = defendTimes;
        }

        protected void AddDefendsToEmbed(List<Models.Tdl.Zone> zones, ref EmbedBuilder embedMsg)
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
                foreach (Models.Tdl.Zone zone in zones)
                {
                    var thisField = new EmbedFieldBuilder
                    {
                        Name = zone.GetDiscordEmbedName(),
                        Value = zone.GetDiscordEmbedValue()
                    };
                    embedMsg.AddField(thisField);
                }
            }
        }

        public EmbedBuilder GetForDate(DateTime date)
        {
            var embedMsg = new EmbedBuilder
            {
                Title = "Defend Schedule for " + date.ToString("dddd, MMM d")
                //Description = ""
            };

            var fromDate = date.ToUniversalTime().AddHours(-date.ToUniversalTime().Hour + 3);
            var todayDefends = _defendTimes.GetNext24Hours(fromDate);

            AddDefendsToEmbed(todayDefends, ref embedMsg);

            return embedMsg;
        }

        public EmbedBuilder GetAll()
        {
            var embedMsg = new EmbedBuilder
            {
                Title = "Full Defend Schedule"
                //Description = ""
            };

            var allDefends = _defendTimes.Zones;

            AddDefendsToEmbed(allDefends, ref embedMsg);

            return embedMsg;
        }
    }
}
