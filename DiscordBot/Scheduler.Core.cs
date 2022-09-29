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