using DiscordBot.Jobs;
using Microsoft.Extensions.Options;
using Quartz;
using Quartz.Impl.AdoJobStore.Common;
using Quartz.Simpl;

namespace DiscordBot;

// TODO: Looks like the scheduler might have a race condition (ie. it might double-stop / double-start)
public partial class Scheduler
{
    public async Task Run(CancellationToken cancellationToken)
    {
        try
        {
            var schedulerTasks = new List<Task>();
            
            /*
            _ramScheduler = await SchedulerBuilder.Create()
                .UseDefaultThreadPool(x => x.MaxConcurrency = _schedulerConfig.RamMaxConcurrency)
                .WithName(_schedulerConfig.RamName)
                .UseInMemoryStore()
                .UseJobFactory<Quartz.Simpl.MicrosoftDependencyInjectionJobFactory>()
                .BuildScheduler();
            */
            _ramScheduler = _schedulerFactory.GetScheduler(cancellationToken).GetAwaiter().GetResult();
            await LoadJobs(cancellationToken);
            _logger.LogInformation("Jobs loaded into RAM");
            schedulerTasks.Add(_ramScheduler.Start(cancellationToken));
            _logger.LogInformation("RAM scheduler started");

            
            _persistentScheduler = await SchedulerBuilder.Create()
                    .UseDedicatedThreadPool(x => x.MaxConcurrency = _schedulerConfig.PersistentMaxConcurrency)
                    .WithName(_schedulerConfig.PersistentName)
                    .UsePersistentStore(x =>
                    {
                        // force job data map values to be considered as strings
                        // prevents nasty surprises if object is accidentally serialized and then 
                        // serialization format breaks, defaults to false
                        x.UseProperties = true;

                        x.UseOracle(_schedulerConfig.PersistentDbConnection);

                        // this requires Quartz.Serialization.Json NuGet package
                        x.UseJsonSerializer();
                    })
                    .BuildScheduler();

            _persistentScheduler.JobFactory = new QuartzJobFactory(_serviceProvider);
            /*
            _persistentScheduler = await SchedulerBuilder.Create()
                .UseDedicatedThreadPool(x => x.MaxConcurrency = _schedulerConfig.PersistentMaxConcurrency)
                .WithName(_schedulerConfig.PersistentName)
                .UsePersistentStore(x =>
                {
                    // force job data map values to be considered as strings
                    // prevents nasty surprises if object is accidentally serialized and then 
                    // serialization format breaks, defaults to false
                    x.UseProperties = true;

                    x.UseOracle(_schedulerConfig.PersistentDbConnection);

                    // this requires Quartz.Serialization.Json NuGet package
                    x.UseJsonSerializer();
                })
                .BuildScheduler();
                */
            //_persistentScheduler = _schedulerFactory.GetScheduler(_schedulerConfig.PersistentName, cancellationToken).GetAwaiter().GetResult();
            schedulerTasks.Add(_persistentScheduler.Start(cancellationToken));
            
            
            
            var jobKey = new JobKey("timer-test");
        
            var triggerKey = new TriggerKey(jobKey.Name);
        
            var job = JobBuilder.Create<TimerDirectMessage>()
                .WithIdentity(jobKey)
                .UsingJobData("userId", "249337230030667777")
                .UsingJobData("message", "Test Timer")
                .Build();

            var jobTriggerBuilder = TriggerBuilder.Create()
                .WithIdentity(triggerKey)
                .StartAt(DateTimeOffset.UtcNow.AddMinutes(2))
                .ForJob(jobKey);

            var jobTrigger = jobTriggerBuilder.Build();

            if (await _persistentScheduler.CheckExists(jobKey))
            {
                await _persistentScheduler.DeleteJob(jobKey);
            }
            await _persistentScheduler.ScheduleJob(job, jobTrigger);
            
            
            Task.WaitAll(schedulerTasks.ToArray(), cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Uncaught exception running scheduler");
        }
    }

    public async Task ReloadJobsAsync(CancellationToken cancellationToken)
    {
        await LoadJobs(cancellationToken);
    }
}