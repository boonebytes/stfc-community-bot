using DiscordBot.Domain.Entities.Alliances;
using DiscordBot.Domain.Entities.Zones;
using DiscordBot.Jobs;
using Quartz;

namespace DiscordBot;

public partial class Scheduler
{
    private async Task LoadJobs(CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Loading jobs");
            await _quartzScheduler.Clear(cancellationToken);
            await AddMonitorJob();
            
            using var thisServiceScope = _serviceProvider.CreateScope();
            var allianceRepository = thisServiceScope.ServiceProvider.GetService<IAllianceRepository>();
            var zoneRepository = thisServiceScope.ServiceProvider.GetService<IZoneRepository>();

            await allianceRepository.InitPostSchedule();
            await zoneRepository.InitZones();

            var alliancesWithSchedule = allianceRepository.GetAllWithServers()
                .Where(a =>
                    a.GuildId.HasValue
                    && a.DefendSchedulePostChannel.HasValue
                    && a.NextScheduledPost.HasValue);

            foreach (var alliance in alliancesWithSchedule)
            {
                if (alliance.NextScheduledPost.HasValue)
                {
                    await AddOrUpdateJob<PostDailySchedule>(alliance.NextScheduledPost.Value.ToUniversalTime(), alliance.Id);
                }

                if (alliance.DefendBroadcastLeadTime.HasValue)
                {
                    foreach (var zone in alliance.Zones)
                    {
                        await AddOrUpdateZoneDefend(thisServiceScope, zone);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception while loading jobs");
        }
    }

    private async Task AddMonitorJob()
    {
        var jobKey = new JobKey("monitor-scheduler");
        
        var triggerKey = new TriggerKey(jobKey.Name);
        
        var job = JobBuilder.Create<MonitorScheduler>()
            .WithIdentity(jobKey)
            .Build();

        var jobTriggerBuilder = TriggerBuilder.Create()
            .WithIdentity(triggerKey)
            .WithDailyTimeIntervalSchedule(s =>
            {
                s.InTimeZone(TimeZoneInfo.Utc);
                s.OnEveryDay();
                s.StartingDailyAt(TimeOfDay.HourAndMinuteOfDay(6, 0));
                s.WithRepeatCount(0);
            })
            .StartNow();
        
        var jobTrigger = jobTriggerBuilder.Build();

        if (await _quartzScheduler.CheckExists(jobKey))
        {
            await _quartzScheduler.DeleteJob(jobKey);
        }
        await _quartzScheduler.ScheduleJob(job, jobTrigger);
    }
    
    /// <summary>
    /// Update the scheduler for the given zone. Obtain the Mutex before calling this!
    /// </summary>
    /// <param name="serviceScope"></param>
    /// <param name="zone"></param>
    private async Task AddOrUpdateZoneDefend(IServiceScope serviceScope, Zone zone)
    {
        await RemoveJob<PostDefendReminder>(0, zone.Id);
        if (zone.Owner != null)
        {
            if (zone.Owner.DefendBroadcastLeadTime.HasValue)
            {
                if (
                    zone.Owner != null
                    && zone.Owner.GuildId.HasValue
                    && zone.Owner.DefendSchedulePostChannel.HasValue
                    && zone.Owner.DefendBroadcastLeadTime.HasValue)
                {
                    zone.SetNextDefend();
                    if (zone.NextDefend.HasValue)
                    {
                        var nextDefend = zone.NextDefend.Value;
                        var nextExecutionTime = nextDefend.AddMinutes(-1 * zone.Owner.DefendBroadcastLeadTime.Value);
                        if (nextExecutionTime < DateTime.Now)
                            nextExecutionTime = nextExecutionTime.AddDays(7);
                        await AddOrUpdateJob<PostDefendReminder>(nextExecutionTime.ToUniversalTime(), zone.Owner.Id, zone.Id);
                    }
                }
            }
        }
    }

    /// <summary>
    /// Remove a job from the scheduler
    /// </summary>
    /// <param name="allianceId"></param>
    /// <param name="itemId"></param>
    private async Task RemoveJob<T>(long allianceId, long itemId = 0) where T : BaseJob
    {
        var jobKey = new JobKey(typeof(T).Name
                                + "-" 
                                + (itemId == 0 ? allianceId : itemId));
        
        if (await _quartzScheduler.CheckExists(jobKey))
        {
            await _quartzScheduler.DeleteJob(jobKey);
        }
    }

    /// <summary>
    /// Add or update a job in the scheduler. Obtain the Mutex before calling this!
    /// </summary>
    /// <param name="triggerTime"></param>
    /// <param name="allianceId"></param>
    /// <param name="itemId"></param>
    /// <param name="isDaily"></param>
    private async Task AddOrUpdateJob<T>(DateTimeOffset triggerTime, long allianceId, long itemId = 0, bool isDaily = true) where T : BaseJob
    {
        _logger.LogInformation($"Adding job {typeof(T).Name} for {triggerTime}, {allianceId}/{itemId}");
        
        var jobKey = new JobKey(typeof(T).Name
                                + "-" 
                                + (itemId == 0 ? allianceId : itemId));
        
        var triggerKey = new TriggerKey(jobKey.Name);
        
        var job = JobBuilder.Create<T>()
            .WithIdentity(jobKey)
            .UsingJobData("allianceId", allianceId)
            .UsingJobData("itemId", itemId)
            .Build();

        var jobTriggerBuilder = TriggerBuilder.Create()
            .WithIdentity(triggerKey)
            .StartAt(triggerTime);

        if (isDaily)
        {
            jobTriggerBuilder.WithDailyTimeIntervalSchedule(s =>
            {
                s.InTimeZone(TimeZoneInfo.Utc);
                s.OnEveryDay();
                s.StartingDailyAt(TimeOfDay.HourAndMinuteOfDay(triggerTime.Hour, triggerTime.Minute));
                s.WithRepeatCount(0);
            });
        }
        else
        {
            jobTriggerBuilder.WithCalendarIntervalSchedule(s =>
            {
                s.InTimeZone(TimeZoneInfo.Utc);
                s.WithIntervalInWeeks(1);
            });
        }
        

        var jobTrigger = jobTriggerBuilder.Build();

        if (await _quartzScheduler.CheckExists(jobKey))
        {
            await _quartzScheduler.DeleteJob(jobKey);
        }
        await _quartzScheduler.ScheduleJob(job, jobTrigger);
        
    }
}