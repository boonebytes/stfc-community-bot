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

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DiscordBot.Domain.Entities.Admin;
using DiscordBot.Domain.Entities.Request;
using DiscordBot.Domain.Seedwork;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DiscordBot.Infrastructure.Repositories
{
    public class CustomMessageJobRepository : BaseRepository, ICustomMessageJobRepository
    {
        public IUnitOfWork UnitOfWork
        {
            get
            {
                return Context;
            }
        }

        public CustomMessageJobRepository(ILogger<CustomMessageJobRepository> logger, BotContext context, RequestContext requestContext) : base(logger, context, requestContext)
        { }

        public CustomMessageJob Add(CustomMessageJob job)
        {
            if (job.IsTransient())
            {
                return Context.CustomMessageJobs
                    .Add(job)
                    .Entity;
            }
            else
            {
                return job;
            }
        }
        
        public CustomMessageJob Update(CustomMessageJob job)
        {
            return Context.CustomMessageJobs
                .Update(job)
                .Entity;
        }

        public async Task<CustomMessageJob> GetAsync(long id)
        {
            var job = await Context.CustomMessageJobs.AsQueryable()
                .Where(a => a.Id == id)
                .SingleOrDefaultAsync();
            return job;
        }

        public async Task<List<CustomMessageJob>> GetAllScheduledAsync()
        {
            return await Context.CustomMessageJobs
                .Where(j => j.Status == JobStatus.Scheduled)
                .ToListAsync();
        }
    }
}
