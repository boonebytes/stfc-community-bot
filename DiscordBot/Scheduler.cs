using DiscordBot.Domain.Entities.Alliances;
using DiscordBot.Domain.Entities.Zones;
using Quartz;

namespace DiscordBot;

public partial class Scheduler
{
    private readonly ILogger<Scheduler> _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly Models.Config.Scheduler _schedulerConfig;
    private readonly ISchedulerFactory _schedulerFactory;
    private IScheduler _ramScheduler;
    private IScheduler _persistentScheduler;

    public Scheduler(
        ILogger<Scheduler> logger,
        IServiceProvider serviceProvider, Models.Config.Scheduler schedulerConfig, ISchedulerFactory schedulerFactory)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
        _schedulerConfig = schedulerConfig;
        _schedulerFactory = schedulerFactory;
    }

    public async Task HandleZoneUpdatedAsync(long zoneId)
    {
        _logger.LogInformation("Zone Update received: {ZoneId}", zoneId);
        
        using var thisServiceScope = _serviceProvider.CreateScope();
        var zoneRepository = thisServiceScope.ServiceProvider.GetService<IZoneRepository>();
        var zone = await zoneRepository.GetAsync(zoneId);
        await AddOrUpdateZoneDefend(thisServiceScope, zone);

        if (zone.Owner != null)
        {
            var allianceRepository = thisServiceScope.ServiceProvider.GetService<IAllianceRepository>();
            var allies = allianceRepository.GetTerritoryHelpersFromOwnerAlliance(zone.Owner.Id);
            
            foreach (var ally in allies)
            {
                await AddOrUpdateZoneAssist(thisServiceScope, zone, ally);
            }
        }
    }
}