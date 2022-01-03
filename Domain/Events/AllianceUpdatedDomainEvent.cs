using System;
using DiscordBot.Domain.Entities.Alliances;
using DiscordBot.Domain.Entities.Zones;
using DiscordBot.Domain.Seedwork;
using MediatR;

namespace DiscordBot.Domain.Events
{
    public class AllianceUpdatedDomainEvent : DomainEvent
    {
        public override DomainEventType EventType => DomainEventType.PostCommit;

        public Alliance Alliance { get; }

        public AllianceUpdatedDomainEvent(Alliance alliance)
        {
            this.Alliance = alliance;
        }
    }
}
