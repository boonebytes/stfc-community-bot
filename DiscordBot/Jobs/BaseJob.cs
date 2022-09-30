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