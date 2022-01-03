using Discord.WebSocket;
using DiscordBot.Domain.Entities.Alliances;
using DiscordBot.Domain.Entities.Zones;
using DiscordBot.Domain.Events;
using DiscordBot.Models.Scheduler;
using MediatR;

namespace DiscordBot;

public partial class Scheduler
{
    private readonly ILogger<Scheduler> _logger;
    private readonly DiscordSocketClient _client;
    private readonly IServiceProvider _serviceProvider;

    private int _pollingIntervalSeconds = 60;

    public Scheduler(
        ILogger<Scheduler> logger,
        DiscordSocketClient client,
        IServiceProvider serviceProvider
    )
    {
        _logger = logger;
        _client = client;
        _serviceProvider = serviceProvider;
    }

    /// <summary>
    /// Update the scheduler for the given zone. Obtain the Mutex before calling this!
    /// </summary>
    /// <param name="serviceScope"></param>
    /// <param name="zone"></param>
    private async Task AddOrUpdateZoneDefend(IServiceScope serviceScope, Zone zone)
    {
        var jobId = nameof(PostDefendReminder) + zone.Id;

        if (zone.Owner != null)
        {
            if (zone.Owner.DefendBroadcastLeadTime.HasValue)
            {
                var job = new PostDefendReminder(_serviceProvider, zone.Id);
                await job.SetNextExecutionTime(serviceScope);
                if (job.NextExecutionTime.HasValue && job.NextExecutionTime.Value > DateTime.Now)
                {
                    AddOrUpdateJob(job);
                    return;
                }
            }
        }

        if (_scheduledJobs.ContainsKey(jobId))
        {
            RemoveJob(jobId);
        }
    }

    private async Task Monitor(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            var now = DateTime.UtcNow;
                
            // 6:00 tomorrow
            var delay = new DateTime(now.Year, now.Month, now.Day, 6, 0, 0, DateTimeKind.Utc).AddDays(1) - DateTime.UtcNow;
            await Task.Delay(delay, cancellationToken);

            if (!cancellationToken.IsCancellationRequested)
            {
                using var thisServiceScope = _serviceProvider.CreateScope();
                var allianceRepository = thisServiceScope.ServiceProvider.GetService<IAllianceRepository>();
                var zoneRepository = thisServiceScope.ServiceProvider.GetService<IZoneRepository>();

                await allianceRepository.InitPostSchedule();
                await zoneRepository.InitZones();
            }
        }
    }
}