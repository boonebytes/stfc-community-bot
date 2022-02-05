using DiscordBot.Domain.Entities.Alliances;
using DiscordBot.Domain.Entities.Zones;
using Quartz;

namespace DiscordBot.Jobs;

public class MonitorScheduler : BaseJob
{
    //private readonly ILogger<MonitorScheduler> _logger;
    private readonly IAllianceRepository _allianceRepository;
    private readonly IZoneRepository _zoneRepository;
    
    public MonitorScheduler(ILogger<MonitorScheduler> logger, IZoneRepository zoneRepository, IAllianceRepository allianceRepository) : base(logger)
    {
        //_logger = logger;
        _zoneRepository = zoneRepository;
        _allianceRepository = allianceRepository;
    }

    protected override async Task DoWork(IJobExecutionContext context)
    {
        _logger.LogInformation("Running the MonitorScheduler job");
        await _allianceRepository.InitPostSchedule();
        await _zoneRepository.InitZones();
    }
}