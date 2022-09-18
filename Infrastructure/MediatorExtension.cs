using System;
using System.Linq;
using System.Threading.Tasks;
using DiscordBot.Domain.Events;
using DiscordBot.Domain.Seedwork;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DiscordBot.Infrastructure
{
    static class MediatorExtension
    {
        public static async Task DispatchDomainEventsAsync(this IMediator mediator, BotContext ctx, ILogger logger, DomainEventType domainEventType)
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
            
            domainEntities
                .ToList()
                .ForEach(entity => entity.Entity.ClearDomainEvents(domainEventType));

            foreach (var domainEvent in domainEvents)
            {
                try
                {
                    
                    await mediator.Publish(domainEvent);
                }
                catch (Exception ex)
                {
                    logger?.LogError(ex, "Unexpected error while calling the post-commit dispatch events");
                }
            }
        }
    }
}
