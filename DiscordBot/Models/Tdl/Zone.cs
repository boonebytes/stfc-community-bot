using System;
using System.Globalization;

namespace DiscordBot.Models.Tdl
{
    public class Zone
    {
        public string Name { get; set; }
        public int Level { get; set; }
        public string Owner { get; set; }
        public string Threats { get; set; }
        public string DefendUtcDayOfWeek { get; set; }
        public string DefendUtcTime { get; set; }
        public string Notes { get; set; }

        public bool LowRisk
        {
            get
            {
                if (Level > 1 && Threats == "")
                    return true;
                else
                    return false;
            }
        }

        protected DateTime? _nextDefend;
        public DateTime NextDefend
        {
            get
            {
                if (_nextDefend.HasValue && _nextDefend > DateTime.UtcNow)
                {
                    return _nextDefend.Value.ToUniversalTime();
                }

                CultureInfo culture = new CultureInfo("en-US");
                DayOfWeek dayOfWeek;
                switch (DefendUtcDayOfWeek.ToLower())
                {
                    case "sunday":
                        dayOfWeek = DayOfWeek.Sunday;
                        break;
                    case "monday":
                        dayOfWeek = DayOfWeek.Monday;
                        break;
                    case "tuesday":
                        dayOfWeek = DayOfWeek.Tuesday;
                        break;
                    case "wednesday":
                        dayOfWeek = DayOfWeek.Wednesday;
                        break;
                    case "thursday":
                        dayOfWeek = DayOfWeek.Thursday;
                        break;
                    case "friday":
                        dayOfWeek = DayOfWeek.Friday;
                        break;
                    case "saturday":
                        dayOfWeek = DayOfWeek.Saturday;
                        break;
                    default:
                        throw new InvalidCastException("Cannot cast day of week " + DefendUtcDayOfWeek);
                }

                DateTime result = DateTime.ParseExact(DefendUtcTime, "h:mm tt", culture, DateTimeStyles.AssumeUniversal).ToUniversalTime();
                if (dayOfWeek != DateTime.UtcNow.DayOfWeek)
                {
                    result = result.AddDays((-1 * (int)DateTime.UtcNow.DayOfWeek) + ((int)dayOfWeek));
                    
                }
                if (result < DateTime.UtcNow) result = result.AddDays(7);
                _nextDefend = result;

                return _nextDefend.Value.ToUniversalTime();
            }
        }

        public string GetDiscordEmbedName()
        {
            return $"{Owner} - {Name} ({Level}^)";
        }

        public string GetDiscordEmbedValue()
        {
            string response = "";

            if (LowRisk)
                response += "__Low Risk__\n";
            response += $"**When**: <t:{NextDefend.ToUniversalTime().Subtract(new DateTime(1970, 1, 1)).TotalSeconds}:t> local / {DefendUtcTime} UTC";
            response += "\n**Threats**: " + (string.IsNullOrEmpty(Threats) ? "None" : Threats);
            if (!string.IsNullOrEmpty(Notes))
            {
                response += $"\n**Notes**: {Notes}";
            }

            return response;
        }
    }
}
