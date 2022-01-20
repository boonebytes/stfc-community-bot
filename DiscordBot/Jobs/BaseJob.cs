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
    
    protected readonly ILogger _logger;

    protected BaseJob(
        ILogger logger)
    {
        _logger = logger;
    }

    protected abstract Task DoWork(IJobExecutionContext context);
    
    public async Task Execute(IJobExecutionContext context)
    {
        try
        {
            await DoWork(context);
        }
        catch (JobExecutionException ex)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                $"Uncaught exception in job {context.JobDetail.Key.ToString()}");
            throw new JobExecutionException(ex, false);
        }
    }
}