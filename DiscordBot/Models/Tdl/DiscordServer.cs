using System;
using System.Globalization;

namespace DiscordBot.Models.Tdl
{
    public class DiscordServer
    {
        public ulong GuildId { get; set; }
        public string AllianceAcronym { get; set; }
        public string AllianceName { get; set; }
        public ulong PostToChannel { get; set; }
        public string PostTime { get; set; }

        protected DateTime? _nextScheduledPost = null;
        public DateTime NextScheduledPost
        {
            get
            {
                if (_nextScheduledPost.HasValue) return _nextScheduledPost.Value;

                CultureInfo culture = new CultureInfo("en-US");
                _nextScheduledPost = DateTime.ParseExact(PostTime, "h:mm tt", culture, DateTimeStyles.AssumeUniversal);
                if (_nextScheduledPost.Value.ToUniversalTime() < DateTime.UtcNow) _nextScheduledPost = _nextScheduledPost.Value.AddDays(1);
                _nextScheduledPost = _nextScheduledPost.Value.ToUniversalTime();
                return _nextScheduledPost.Value;
            }
        }

        public void FlagPosted()
        {
            _nextScheduledPost = null;
        }
    }
}
