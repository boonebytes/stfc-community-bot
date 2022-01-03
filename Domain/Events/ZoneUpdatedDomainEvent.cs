﻿using System;
using DiscordBot.Domain.Entities.Zones;
using DiscordBot.Domain.Seedwork;
using MediatR;

namespace DiscordBot.Domain.Events
{
    public class ZoneUpdatedDomainEvent : DomainEvent
    {
        public override DomainEventType EventType => DomainEventType.PostCommit;

        public Zone Zone { get; }

        public ZoneUpdatedDomainEvent(Zone zone)
        {
            this.Zone = zone;
        }
    }
}
