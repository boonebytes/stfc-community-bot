using System;
namespace DiscordBot.Domain.Shared
{
    public static class Functions
    {
        public static DateTime ToEasternTime(this DateTime dateTime)
        {
            var zone = TimeZoneInfo.FindSystemTimeZoneById("America/New_York");

            return TimeZoneInfo.ConvertTimeFromUtc(dateTime.ToUniversalTime(), zone);
        }

        public static long ToUnixTimestamp(this DateTime dateTime)
        {
            return (long) Math.Round(dateTime.ToUniversalTime().Subtract(new DateTime(1970, 1, 1)).TotalSeconds, 0);
        }
    }
}
