using DiscordBot.Models.Scheduler;

namespace DiscordBot;

public partial class Scheduler
{
    private Mutex _scheduledJobsMutex = new();
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

    private void StartScheduler()
    {
        _scheduleRunnerStopTokenSource = new CancellationTokenSource();
        _scheduleRunnerStopToken = _scheduleRunnerStopTokenSource.Token;

        _scheduleRunner = ScheduleRunner(_scheduleRunnerStopToken);
    }

    private void StopScheduler(CancellationToken cancellationToken)
    {
        _scheduleRunnerStopTokenSource.Cancel();
        _scheduleRunner.Wait(cancellationToken);
    }

    private async Task ScheduleRunner(CancellationToken cancellationToken)
    {
        _scheduledJobsMutex.WaitOne();
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
                    await Task.Delay(delay, cancellationToken);
                }
                else if (nextJob.NextExecutionTime.HasValue)
                {
                    if (nextJob.NextExecutionTime.Value > DateTime.UtcNow)
                    {
                        var delay = nextJob.NextExecutionTime.Value - DateTime.UtcNow;
                        if (delay.TotalSeconds > _pollingIntervalSeconds)
                            delay = new TimeSpan(0, 0, _pollingIntervalSeconds);
                        await Task.Delay(delay, cancellationToken);
                    }

                    if (!cancellationToken.IsCancellationRequested && DateTime.UtcNow >= nextJob.NextExecutionTime)
                    {
                        _ = Task.Run(async () =>
                        {
                            var beforeExecTime = nextJob.NextExecutionTime.Value;
                            nextJob.IsCurrentlyRunning = true;
                            await nextJob.DoWork(cancellationToken);

                            // Try to detect a problem with the next execution time not being set to the future
                            if (nextJob.NextExecutionTime.HasValue && nextJob.NextExecutionTime.Value <= beforeExecTime)
                            {
                                nextJob.NextExecutionTime = null;
                            }

                            // Wait 30 seconds after the job, to prevent something going seriously wrong
                            // and causing the same job to repeat itself infinitely.
                            var delay = new TimeSpan(0, 0, 30);
                            await Task.Delay(delay, cancellationToken);
                            nextJob.IsCurrentlyRunning = false;
                        }, cancellationToken);
                    }
                }
            }
        }
        finally
        {
            _scheduledJobsMutex.ReleaseMutex();
        }
    }
}