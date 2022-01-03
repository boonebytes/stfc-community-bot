using System;
using DiscordBot.Domain.Entities.Alliances;
using DiscordBot.Domain.Events;
using DiscordBot.Domain.Seedwork;

namespace DiscordBot.Domain.Entities.Zones
{
    public partial class Zone : IAggregateRoot
    {
        public Zone(
                long id,
                string name,
                int level,
                Alliance owner,
                string threats,
                string defendUtcDayOfWeek,
                string defendUtcTime,
                string notes
            )
        {
            Id = id;
            this.Update(
                name,
                level,
                owner,
                threats,
                defendUtcDayOfWeek,
                defendUtcTime,
                notes
                );
        }

        public void Update(
                string name,
                int level,
                Alliance owner,
                string threats,
                string defendUtcDayOfWeek,
                string defendUtcTime,
                string notes
            )
        {
            if (threats == "") threats = null;

            Name = name;
            if (level >= 1 && level <= 3)
                Level = level;

            Owner = owner;
            Threats = threats;
            DefendUtcDayOfWeek = defendUtcDayOfWeek;
            DefendUtcTime = defendUtcTime;
            Notes = notes;

            AddZoneChangedDomainEvent();
        }
    }
}
