using System;
using System.Linq;
using System.Threading.Tasks;
using DiscordBot.Domain.Events;
using DiscordBot.Domain.Seedwork;
using MediatR;

namespace DiscordBot.Infrastructure
{
    static class MediatorExtension
    {
        public static async Task DispatchDomainEventsAsync(this IMediator mediator, BotContext ctx, DomainEventType domainEventType)
        {
            var domainEntities = ctx.ChangeTracker
                .Entries<Entity>()
                .Where(x =>
                    x.Entity.DomainEvents != null
                    && x.Entity.DomainEvents.Any());

            var domainEvents = domainEntities
                .SelectMany(x => x.Entity.DomainEvents)
                .Where(e => e.EventType == domainEventType)
                .ToList();

            domainEntities.ToList()
                .ForEach(entity => entity.Entity.ClearDomainEvents(domainEventType));

            foreach (var domainEvent in domainEvents)
            {
                if (domainEventType == DomainEventType.PreCommit)
                {
                    await mediator.Publish(domainEvent);
                }
                else
                {
                    _ = mediator.Publish(domainEvent);
                }
            }
        }
    }
}
