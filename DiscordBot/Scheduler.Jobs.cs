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

using DiscordBot.Domain.Entities.Admin;
using DiscordBot.Domain.Entities.Alliances;
using DiscordBot.Domain.Entities.Zones;
using DiscordBot.Jobs;
using Quartz;

namespace DiscordBot;

public partial class Scheduler
{
    private JobKey GetJobKey<T>(long allianceId, long itemId = 0) where T : BaseJob
    {
        if (typeof(T) == typeof(PostDefendReminder))
        {
            return new JobKey(typeof(T).Name + "-" + itemId);
        }
        else
        {
            return new JobKey(typeof(T).Name
                                    + "-" 
                                    + (itemId == 0 ? allianceId : itemId + "-" + allianceId));
        }
        
    }
    
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
                    var nextPost = alliance.NextScheduledPost.Value.ToUniversalTime();
                    //if (nextPost < DateTime.Now.AddMinutes(5).ToUniversalTime())
                    //    nextPost = DateTime.Now.AddMinutes(5).ToUniversalTime();
                    await AddOrUpdateJob<PostDailySchedule>(nextPost, alliance.Id);
                }

                if (alliance.DefendBroadcastLeadTime.HasValue)
                {
                    foreach (var zone in alliance.Zones)
                    {
                        await AddOrUpdateZoneDefend(thisServiceScope, zone);
                    }

                    if (alliance.AlliedBroadcastRole.HasValue)
                    {
                        for (var i = 0; i < 7; i++)
                        {
                            var dow = (DayOfWeek)i;
                            var interestedDefends = zoneRepository.GetFromDayOfWeek(dow, alliance.Id);
                            foreach (var zone in interestedDefends)
                            {
                                await AddOrUpdateZoneAssist(thisServiceScope, zone, alliance);
                            }
                        }
                        
                    }
                }
            }

            await LoadCustomMessageJobs(thisServiceScope);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception while loading jobs");
        }
    }

    private async Task LoadCustomMessageJobs(IServiceScope thisServiceScope)
    {
        var customMessagesRepository = thisServiceScope.ServiceProvider.GetService<ICustomMessageJobRepository>();
        var customMessageJobs = await customMessagesRepository.GetAllScheduledAsync();
        foreach (var job in customMessageJobs)
        {
            await AddOrUpdateJob<PostCustomMessage>(job.ScheduledTimestamp, job.Alliance.Id, job.Id);
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
            .WithSchedule(CronScheduleBuilder
                .DailyAtHourAndMinute(6,0)
                .InTimeZone(TimeZoneInfo.Utc)
            )
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
                    && zone.Owner.DefendSchedulePostChannel.HasValue)
                {
                    zone.SetNextDefend();
                    if (zone.NextDefend.HasValue)
                    {
                        var nextDefend = zone.NextDefend.Value;
                        var nextExecutionTime = nextDefend.AddMinutes(-1 * zone.Owner.DefendBroadcastLeadTime.Value);
                        if (nextExecutionTime < DateTime.Now)
                            nextExecutionTime = nextExecutionTime.AddDays(7);
                        await AddOrUpdateJob<PostDefendReminder>(nextExecutionTime.ToUniversalTime(), zone.Owner.Id, zone.Id, false);
                    }
                }
            }
        }
    }

    /// <summary>
    /// Update the scheduler for the given zone. Obtain the Mutex before calling this!
    /// </summary>
    /// <param name="serviceScope"></param>
    /// <param name="zone"></param>
    /// <param name="alliance"></param>
    private async Task AddOrUpdateZoneAssist(IServiceScope serviceScope, Zone zone, Alliance alliance)
    {
        await RemoveJob<PostAssistReminder>(alliance.Id,zone.Id);
        if (zone.Owner != null && zone.Owner != alliance)
        {
            if (alliance.DefendBroadcastLeadTime.HasValue)
            {
                if (
                    alliance.GuildId.HasValue
                    && alliance.DefendSchedulePostChannel.HasValue)
                {
                    zone.SetNextDefend();
                    if (zone.NextDefend.HasValue)
                    {
                        var nextDefend = zone.NextDefend.Value;
                        var nextExecutionTime = nextDefend.AddMinutes(-1 * alliance.DefendBroadcastLeadTime.Value);
                        if (nextExecutionTime < DateTime.Now)
                            nextExecutionTime = nextExecutionTime.AddDays(7);
                        await AddOrUpdateJob<PostAssistReminder>(nextExecutionTime.ToUniversalTime(), alliance.Id, zone.Id, false);
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
        var jobKey = GetJobKey<T>(allianceId, itemId);
        
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
        _logger.LogInformation("Adding job {JobType} for {TriggerTime}, {AllianceId}/{ItemId}",
            typeof(T).Name, triggerTime, allianceId, itemId);

        var jobKey = GetJobKey<T>(allianceId, itemId);
        
        var triggerKey = new TriggerKey(jobKey.Name);
        
        var job = JobBuilder.Create<T>()
            .WithIdentity(jobKey)
            .UsingJobData("allianceId", allianceId)
            .UsingJobData("itemId", itemId)
            .Build();

        
        var jobTriggerBuilder = TriggerBuilder.Create()
            .WithIdentity(triggerKey)
            .StartAt(triggerTime)
            .ForJob(jobKey);

        if (isDaily)
        {
            jobTriggerBuilder.WithSchedule(CronScheduleBuilder
                .DailyAtHourAndMinute(triggerTime.Hour, triggerTime.Minute)
                .InTimeZone(TimeZoneInfo.Utc)
            );
            /*
            jobTriggerBuilder.WithDailyTimeIntervalSchedule(s =>
            {
                s.InTimeZone(TimeZoneInfo.Utc);
                s.OnEveryDay();
                s.StartingDailyAt(TimeOfDay.HourAndMinuteOfDay(triggerTime.Hour, triggerTime.Minute));
                s.WithRepeatCount(-1);
            });
            */
        }
        else
        {
            jobTriggerBuilder.WithSchedule(CronScheduleBuilder
                .WeeklyOnDayAndHourAndMinute(triggerTime.DayOfWeek, triggerTime.Hour, triggerTime.Minute)
                .InTimeZone(TimeZoneInfo.Utc)
            );
            /*
            jobTriggerBuilder.WithCalendarIntervalSchedule(s =>
            {
                s.InTimeZone(TimeZoneInfo.Utc);
                s.WithIntervalInWeeks(1);
            });
            */
        }
        

        var jobTrigger = jobTriggerBuilder.Build();

        if (await _quartzScheduler.CheckExists(jobKey))
        {
            await _quartzScheduler.DeleteJob(jobKey);
        }
        await _quartzScheduler.ScheduleJob(job, jobTrigger);
        
    }
}