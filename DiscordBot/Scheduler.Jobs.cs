using DiscordBot.Domain.Entities.Alliances;
using DiscordBot.Domain.Entities.Zones;
using DiscordBot.Models.Scheduler;

namespace DiscordBot;

public partial class Scheduler
{
    private async Task LoadJobs(CancellationToken cancellationToken)
    {
        bool acquiredLock = false;
        try
        {
            await _scheduledJobsSemaphore.WaitAsync(cancellationToken);
            acquiredLock = true;
        }
        catch (OperationCanceledException ex)
        {
            _logger.LogWarning(ex, "Operation Cancelled");
        }

        if (acquiredLock)
        {
            try
            {
                _scheduledJobs.Clear();

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
                        var newJob = new PostDailySchedule(_serviceProvider, alliance.Id)
                        {
                            NextExecutionTime = alliance.NextScheduledPost.Value
                        };
                        AddOrUpdateJob(newJob);
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
            catch (AbandonedMutexException ex)
            {
                _logger.LogError(ex, "Abandoned mutex");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while loading jobs");
                throw;
            }
            finally
            {
                _scheduledJobsSemaphore.Release(1);
            }
        }
    }

    /// <summary>
    /// Remove a job from the scheduler. Obtain the Mutex before calling this!
    /// </summary>
    /// <param name="jobId"></param>
    private void RemoveJob(string jobId)
    {
        if (_scheduledJobs.ContainsKey(jobId))
        {
            _scheduledJobs.Remove(jobId);
        }
    }

    /// <summary>
    /// Add or update a job in the scheduler. Obtain the Mutex before calling this!
    /// </summary>
    /// <param name="job"></param>
    private void AddOrUpdateJob(BaseJob job)
    {
        if (_scheduledJobs.ContainsKey(job.Id))
        {
            _scheduledJobs[job.Id] = job;
        }
        else
        {
            _scheduledJobs.Add(job.Id, job);
        }
    }
}