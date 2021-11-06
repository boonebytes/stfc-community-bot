using System;
using DiscordBot.Domain.Seedwork;

namespace DiscordBot.Domain.Entities.Zones
{
    public class ZoneNeighbour : Entity
    {
        private long _fromZoneId;
        public virtual Zone FromZone { get; private set; }

        private long _toZoneId;
        public virtual Zone ToZone { get; private set; }

        public ZoneNeighbour() { }

        public ZoneNeighbour(Zone fromZone, Zone toZone)
        {
            _fromZoneId = fromZone.Id;
            _toZoneId = toZone.Id;
            FromZone = fromZone;
            ToZone = toZone;
        }
    }
}
