using System;
namespace DiscordBot.Domain.Shared
{
    public static class Functions
    {
        public static DateTime ToEasternTime(this DateTime dateTime)
        {
            //foreach (TimeZoneInfo z in TimeZoneInfo.GetSystemTimeZones())
            //    Console.WriteLine(z.Id);

            var zone = TimeZoneInfo.FindSystemTimeZoneById("America/New_York");

            return TimeZoneInfo.ConvertTimeFromUtc(dateTime.ToUniversalTime(), zone);
            /*
            TimeZoneInfo zone;
            DateTime startDate;
            DateTime endDate;
            DateTime src = dateTime.ToUniversalTime();

            zone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
            TimeZoneInfo.AdjustmentRule[] rules = zone.GetAdjustmentRules();

            TimeSpan timeSpan = new TimeSpan();

            foreach (TimeZoneInfo.AdjustmentRule rule in rules)
            {
                if (src.CompareTo(rule.DateStart) > 0 && src.CompareTo(rule.DateEnd) < 0)
                {
                    startDate = GetDateTime(src.Year, rule.DaylightTransitionStart);
                    endDate = GetDateTime(src.Year, rule.DaylightTransitionEnd);

                    if (src.CompareTo(startDate) > 0 && src.CompareTo(endDate) < 0)
                    {
                        timeSpan = rule.DaylightDelta;
                        break;
                    }
                }
            }

            DateTime finalDate = src.Add(timeSpan);
            */
        }
    }
}
