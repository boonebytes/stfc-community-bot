/*
Copyright 2022 Boonebytes

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/

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
