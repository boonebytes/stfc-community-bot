using Discord.WebSocket;
using DiscordBot.Domain.Entities.Zones;
using Quartz;
using Quartz.Impl;
using Quartz.Logging;

namespace DiscordBot;

public partial class Scheduler
{
    private readonly ILogger<Scheduler> _logger;
    private readonly DiscordSocketClient _client;
    private readonly IServiceProvider _serviceProvider;
    private IScheduler _quartzScheduler = null;

    public Scheduler(
        ILogger<Scheduler> logger,
        DiscordSocketClient client,
        IServiceProvider serviceProvider
    )
    {
        _logger = logger;
        _client = client;
        _serviceProvider = serviceProvider;
        //_quartzScheduler = quartzScheduler;
        //LogProvider.SetCurrentLogProvider(new QuartzLogger(_serviceProvider.GetService<ILogger<QuartzLogger>>()));
    }

    public async Task HandleZoneUpdatedAsync(long zoneId)
    {
        _logger.LogInformation($"Zone Update received: {zoneId}");
        
        using var thisServiceScope = _serviceProvider.CreateScope();
        var zoneRepository = thisServiceScope.ServiceProvider.GetService<IZoneRepository>();
        var zone = await zoneRepository.GetAsync(zoneId, null);
        await AddOrUpdateZoneDefend(thisServiceScope, zone);
    }
}