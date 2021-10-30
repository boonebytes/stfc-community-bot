using System;
using System.Collections.Generic;
using System.Globalization;
using DiscordBot.Domain.Seedwork;

namespace DiscordBot.Domain.Entities.Alliances
{
    public partial class Alliance : Entity, IAggregateRoot
    {
        public Alliance()
        {
        }

        public virtual string Name { get; private set; }
        public virtual string Acronym { get; private set; }

        private readonly List<Diplomacy> _diplomacy;
        public IReadOnlyCollection<Diplomacy> Diplomacy => _diplomacy;

        public virtual ulong? GuildId { get; protected set; }
        public virtual ulong? DefendSchedulePostChannel { get; protected set; }
        public virtual string DefendSchedulePostTime { get; protected set; }

        protected DateTime? _nextScheduledPost = null;
        public DateTime NextScheduledPost
        {
            get
            {
                if (_nextScheduledPost.HasValue) return _nextScheduledPost.Value;

                CultureInfo culture = new CultureInfo("en-US");
                _nextScheduledPost = DateTime.ParseExact(DefendSchedulePostTime, "h:mm tt", culture, DateTimeStyles.AssumeUniversal);
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
