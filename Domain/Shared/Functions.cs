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

using System;

namespace DiscordBot.Domain.Shared
{
    public static class Functions
    {
        public static DateTime ToEasternTime(this DateTime dateTime)
        {
            TimeZoneInfo zone = null;
            try
            {
                zone = TimeZoneInfo.FindSystemTimeZoneById("America/New_York");
            }
            catch (TimeZoneNotFoundException ex)
            {
                // This block may be reached on Windows hosts
                // TODO: Verify this respects DST adjustments
                zone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
            }
            
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
