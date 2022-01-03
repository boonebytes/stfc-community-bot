using DiscordBot.Domain.Entities.Zones;
using DiscordBot.Domain.Events;
using MediatR;

namespace DiscordBot;

public partial class Scheduler :
    INotificationHandler<ZoneUpdatedDomainEvent>,
    INotificationHandler<AllianceUpdatedDomainEvent>
{
    public async Task Handle(AllianceUpdatedDomainEvent notification, CancellationToken cancellationToken)
    {
        StopScheduler(cancellationToken);
        if (!cancellationToken.IsCancellationRequested) await LoadJobs(cancellationToken);
        if (!cancellationToken.IsCancellationRequested) StartScheduler();
    }
    
    public async Task Handle(ZoneUpdatedDomainEvent notification, CancellationToken cancellationToken)
    {
        if (notification.Zone == null) return;

        using var thisServiceScope = _serviceProvider.CreateScope();
        //var allianceRepository = thisServiceScope.ServiceProvider.GetService<IAllianceRepository>();
        var zoneRepository = thisServiceScope.ServiceProvider.GetService<IZoneRepository>();

        var zone = await zoneRepository.GetAsync(notification.Zone.Id);

        StopScheduler(cancellationToken);
        if (!cancellationToken.IsCancellationRequested)
        {
            _scheduledJobsMutex.WaitOne();
            try
            {
                await AddOrUpdateZoneDefend(thisServiceScope, zone);
                StartScheduler();
            }
            finally
            {
                _scheduledJobsMutex.ReleaseMutex();
            }
        }
    }
}