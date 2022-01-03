namespace DiscordBot.Models.Scheduler;

public abstract class BaseJob
{
    public abstract string Id { get; }
    public bool IsCurrentlyRunning { get; set; }
    public DateTime? NextExecutionTime { get; set; }
    public abstract Task DoWork(CancellationToken cancellationToken);

    protected readonly IServiceProvider _serviceProvider;
        
    protected BaseJob(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public abstract Task SetNextExecutionTime(IServiceScope serviceScope);
}