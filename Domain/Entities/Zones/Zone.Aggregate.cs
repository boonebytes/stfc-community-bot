using DiscordBot.Domain.Entities.Alliances;
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
                defendUtcDayOfWeek,
                defendUtcTime,
                notes
                );
        }

        public void Update(
                string name,
                int level,
                Alliance owner,
                string defendUtcDayOfWeek,
                string defendUtcTime,
                string notes
            )
        {
            Name = name;
            if (level >= 1 && level <= 3)
                Level = level;

            Owner = owner;
            DefendUtcDayOfWeek = defendUtcDayOfWeek;
            DefendUtcTime = defendUtcTime;
            Notes = notes;

            AddZoneChangedDomainEvent();
        }
    }
}
