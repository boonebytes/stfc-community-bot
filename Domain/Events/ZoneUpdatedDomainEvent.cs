using System;
using DiscordBot.Domain.Entities.Zones;
using MediatR;

namespace DiscordBot.Domain.Events
{
    public class ZoneUpdatedDomainEvent
        : INotification
    {
        public Zone Zone { get; set; }

        public ZoneUpdatedDomainEvent(Zone zone)
        {
            this.Zone = zone;
        }
    }
}
