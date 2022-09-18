using Quartz;

namespace DiscordBot.Jobs;

public abstract class BaseJob : IJob
{
    /*
    public static class JobKeyPrefixes
    {
        public const string PostDailySchedule = "PostDailySchedule";
    }
    */
    
    protected readonly ILogger Logger;

    protected BaseJob(
        ILogger logger)
    {
        Logger = logger;
    }

    protected abstract Task DoWork(IJobExecutionContext context);
    
    public async Task Execute(IJobExecutionContext context)
    {
        try
        {
            await DoWork(context);
        }
        catch (JobExecutionException)
        {
            throw;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex,
                "Uncaught exception in job {JobId}",
                context.JobDetail.Key.ToString());
            throw new JobExecutionException(ex, false);
        }
    }
}