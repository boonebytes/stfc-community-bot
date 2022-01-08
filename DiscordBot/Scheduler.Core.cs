using DiscordBot.Models.Scheduler;

namespace DiscordBot;

// TODO: Looks like the scheduler might have a race condition (ie. it might double-stop / double-start)
public partial class Scheduler
{
    private readonly SemaphoreSlim _scheduledJobsSemaphore = new(1, 1);
    private readonly Dictionary<string, BaseJob> _scheduledJobs = new();
    private CancellationTokenSource _scheduleRunnerStopTokenSource;
    private CancellationToken _scheduleRunnerStopToken;
    private Task _scheduleRunner = null;

    public async Task Run(int pollingIntervalSeconds, CancellationToken cancellationToken)
    {
        _pollingIntervalSeconds = pollingIntervalSeconds;
        await LoadJobs(cancellationToken);
        _ = Monitor(cancellationToken);

        StartScheduler();

        var cancelHandle = cancellationToken.WaitHandle;
        cancelHandle.WaitOne();

        StopScheduler(CancellationToken.None);
    }

    public async Task RestartSchedulerAsync(CancellationToken cancellationToken)
    {
        try
        {
            StopScheduler(cancellationToken);
            await LoadJobs(cancellationToken);
            StartScheduler();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while restarting the scheduler");
        }
    }

    private void StartScheduler()
    {
        _scheduleRunnerStopTokenSource = new CancellationTokenSource();
        _scheduleRunnerStopToken = _scheduleRunnerStopTokenSource.Token;

        _scheduleRunner = ScheduleRunner(_scheduleRunnerStopToken);
        _logger.LogInformation("Scheduler started");
    }

    private void StopScheduler(CancellationToken cancellationToken)
    {
        _scheduleRunnerStopTokenSource.Cancel();
        _scheduleRunner.Wait(cancellationToken);
        _logger.LogInformation("Scheduler stopped");
    }

    private async Task ScheduleRunner(CancellationToken cancellationToken)
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
                while (!cancellationToken.IsCancellationRequested)
                {
                    var nextJob = _scheduledJobs.Select(i => i.Value)
                        .Where(j =>
                            j.NextExecutionTime.HasValue
                            && !j.IsCurrentlyRunning)
                        .OrderBy(j => j.NextExecutionTime.Value)
                        .FirstOrDefault();
                    if (nextJob == null)
                    {
                        var delay = new TimeSpan(0, 0, _pollingIntervalSeconds);
                        await Task.Delay(delay, cancellationToken)
                            .ContinueWith(t => { }, cancellationToken: CancellationToken.None);
                    }
                    else if (nextJob.NextExecutionTime.HasValue)
                    {
                        if (nextJob.NextExecutionTime.Value > DateTime.UtcNow)
                        {
                            var delay = nextJob.NextExecutionTime.Value - DateTime.UtcNow;
                            if (delay.TotalSeconds > _pollingIntervalSeconds)
                                delay = new TimeSpan(0, 0, _pollingIntervalSeconds);
                            await Task.Delay(delay, cancellationToken)
                                .ContinueWith(t => { }, cancellationToken: CancellationToken.None);
                        }

                        if (!cancellationToken.IsCancellationRequested && DateTime.UtcNow >= nextJob.NextExecutionTime)
                        {
                            nextJob.IsCurrentlyRunning = true;
                            _logger.LogInformation($"Running job {nextJob.Id}");
                            var beforeExecTime = nextJob.NextExecutionTime.Value;
                            await nextJob.DoWork(cancellationToken);

                            // Try to detect a problem with the next execution time not being set to the future
                            if (nextJob.NextExecutionTime.HasValue && nextJob.NextExecutionTime.Value <= beforeExecTime)
                            {
                                _logger.LogWarning(
                                    $"Job {nextJob.Id} - Next execution time either blank or in the past. Preventing job from running.");
                                nextJob.NextExecutionTime = null;
                            }

                            // Wait 30 seconds after the job, to prevent something going seriously wrong
                            // and causing the same job to repeat itself infinitely.
                            var delay = new TimeSpan(0, 0, 30);
                            await Task.Delay(delay, cancellationToken)
                                .ContinueWith(t => { }, cancellationToken: CancellationToken.None);;
                            nextJob.IsCurrentlyRunning = false;
                            AddOrUpdateJob(nextJob);
                        }
                    }
                }
            }
            catch (TaskCanceledException ex)
            {
                _logger.LogInformation(ex, "Task Cancel caught in Schedule Runner");
            }
            catch (AbandonedMutexException ex)
            {
                _logger.LogError(ex, "Abandoned mutex");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while running the core scheduler");
            }
            finally
            {
                _scheduledJobsSemaphore.Release(1);
            }
        }
    }
}