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
            return (long) Math.Round(dateTime.Subtract(new DateTime(1970, 1, 1)).TotalSeconds, 0);
        }

        public static string FriendlyNumberFormat(long num)
        {
            num = MaxThreeSignificantDigits(num);

            switch (num)
            {
                case >= 100000000:
                    return (num / 1000000D).ToString("0.## M");
                case >= 10000000:
                    return (num / 1000000D).ToString("0.# M");
                case >= 1000000:
                    return (num / 1000000D).ToString("0.## M");
                case >= 100000:
                    return (num / 1000D).ToString("0 K");
                case >= 10000:
                    return (num / 1000D).ToString("0.# K");
                case >= 1000:
                    return (num / 1000D).ToString("0.## K");
                default:
                    return num.ToString("#,0");
            }
        }

        private static long MaxThreeSignificantDigits(long x)
        {
            int i = (int)Math.Log10(x);
            i = Math.Max(0, i - 2);
            i = (int)Math.Pow(10, i);
            return x / i * i;
        }
    }
}
