using DiscordBot.Domain.Events;
using MediatR;

namespace DiscordBot.Domain.Seedwork
{
    public abstract class DomainEvent : INotification
    {
        public abstract DomainEventType EventType { get; }
    }
}