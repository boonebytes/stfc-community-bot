using Quartz;

namespace DiscordBot;

// TODO: Looks like the scheduler might have a race condition (ie. it might double-stop / double-start)
public partial class Scheduler
{
    public async Task Run(CancellationToken cancellationToken)
    {
        
        _quartzScheduler = await SchedulerBuilder.Create()
            .UseDefaultThreadPool(x => x.MaxConcurrency = 5)
            .BuildScheduler();
        
        
        await LoadJobs(cancellationToken);
        await _quartzScheduler.Start(cancellationToken);
    }

    public async Task ReloadJobsAsync(CancellationToken cancellationToken)
    {
        await LoadJobs(cancellationToken);
    }
}