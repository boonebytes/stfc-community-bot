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
                int currentLine = 0;
                foreach (Models.Tdl.Zone zone in zones)
                {
                    if (currentLine > 0)
                    {
                        //embedMsg.AddField("\u200B", "\u200B", true);
                        //embedMsg.AddField("\b", "\b", true);
                        //embedMsg.AddField("\u200b", "\u200b", true);
                    }
                    currentLine++;
                    var thisField = new EmbedFieldBuilder
                    {
                        Name = zone.GetDiscordEmbedName(),
                        Value = zone.GetDiscordEmbedValue() + "\n\u200b"
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

            var fromDate = date.ToUniversalTime();
            if (fromDate.Hour < 4)
            {
                fromDate = fromDate.AddDays(-1);
            }
            fromDate = fromDate.AddHours(-date.ToUniversalTime().Hour + 3);

            var todayDefends = _defendTimes.GetNext24Hours(fromDate).OrderBy(z => z.NextDefend).ToList();

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

            var allDefends = _defendTimes.Zones.OrderBy(z => z.NextDefend).ToList();

            AddDefendsToEmbed(allDefends, ref embedMsg);

            return embedMsg;
        }
    }
}
