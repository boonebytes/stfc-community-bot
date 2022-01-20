using DiscordBot.Domain.Entities.Zones;
using DiscordBot.Domain.Events;
using MediatR;

namespace DiscordBot;

public class SchedulerHandler :
    INotificationHandler<ZoneUpdatedDomainEvent>,
    INotificationHandler<AllianceUpdatedDomainEvent>
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
}