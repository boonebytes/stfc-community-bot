using System;
using System.Collections.Generic;
using System.Globalization;
using DiscordBot.Domain.Entities.Zones;
using DiscordBot.Domain.Events;
using DiscordBot.Domain.Seedwork;

namespace DiscordBot.Domain.Entities.Alliances
{
    public partial class Alliance : Entity
    {
        public Alliance()
        {
        }

        public virtual string Name { get; private set; }
        public virtual string Acronym { get; private set; }

        private long? _allianceGroupId;
        public virtual AllianceGroup Group { get; private set; }

        private readonly List<Diplomacy> _assignedDiplomacy;
        public IReadOnlyCollection<Diplomacy> AssignedDiplomacy => _assignedDiplomacy;

        private readonly List<Diplomacy> _receivedDiplomacy;
        public IReadOnlyCollection<Diplomacy> ReceivedDiplomacy => _receivedDiplomacy;

        public virtual ulong? GuildId { get; protected set; }
        public virtual ulong? DefendSchedulePostChannel { get; protected set; }
        public virtual string DefendSchedulePostTime { get; protected set; }
        public virtual int? DefendBroadcastLeadTime { get; protected set; }

        public void SetDefendBroadcastLeadTime(int? value)
        {
            DefendBroadcastLeadTime = value;
            AddAllianceChangedDomainEvent();
        }

        private readonly List<Zone> _zones;
        public IReadOnlyCollection<Zone> Zones => _zones;

        public virtual DateTime? NextScheduledPost { get; private set; }

        public void SetNextScheduledPost()
        {
            if (GuildId.HasValue && DefendSchedulePostChannel.HasValue && !string.IsNullOrEmpty(DefendSchedulePostTime))
            {
                CultureInfo culture = new CultureInfo("en-US");
                NextScheduledPost = DateTime.ParseExact(DefendSchedulePostTime, "h:mm tt", culture, DateTimeStyles.AssumeUniversal);
                if (NextScheduledPost.Value.ToUniversalTime() < DateTime.UtcNow) NextScheduledPost = NextScheduledPost.Value.AddDays(1);
                NextScheduledPost = NextScheduledPost.Value.ToUniversalTime();
            }
            else
            {
                NextScheduledPost = null;
            }
        }

        public void FlagPosted()
        {
            SetNextScheduledPost();
        }
        
        public void AddAllianceChangedDomainEvent()
        {
            this.AddDomainEvent(new AllianceUpdatedDomainEvent(this));
        }
    }
}
