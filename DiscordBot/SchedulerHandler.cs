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

using DiscordBot.Domain.Events;
using MediatR;

namespace DiscordBot;

public class SchedulerHandler :
    INotificationHandler<ZoneUpdatedDomainEvent>,
    INotificationHandler<AllianceUpdatedDomainEvent>,
    INotificationHandler<CustomMessageJobUpdatedDomainEvent>
{
    private readonly ILogger<SchedulerHandler> _logger;
    private readonly Scheduler _scheduler;

    public SchedulerHandler(ILogger<SchedulerHandler> logger, Scheduler scheduler)
    {
        _logger = logger;
        _scheduler = scheduler;
    }
    
    public async Task Handle(AllianceUpdatedDomainEvent notification, CancellationToken cancellationToken)
    {
        _ = _scheduler.ReloadJobsAsync(CancellationToken.None);
    }
    
    public async Task Handle(ZoneUpdatedDomainEvent notification, CancellationToken cancellationToken)
    {
        if (notification.Zone != null)
            _ = _scheduler.HandleZoneUpdatedAsync(notification.Zone.Id);
    }

    public async Task Handle(CustomMessageJobUpdatedDomainEvent notification, CancellationToken cancellationToken)
    {
        if (notification.Job != null)
            _ = _scheduler.HandleCustomMessageJobUpdatedAsync(notification.Job.Id);
    }
}