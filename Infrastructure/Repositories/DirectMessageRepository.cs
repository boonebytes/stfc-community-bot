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

using System.Linq;
using System.Threading.Tasks;
using DiscordBot.Domain.Entities.Admin;
using DiscordBot.Domain.Entities.Request;
using DiscordBot.Domain.Seedwork;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DiscordBot.Infrastructure.Repositories
{
    public class DirectMessageRepository : BaseRepository, IDirectMessageRepository
    {
        public IUnitOfWork UnitOfWork
        {
            get
            {
                return Context;
            }
        }

        public DirectMessageRepository(ILogger<DirectMessageRepository> logger, BotContext context, RequestContext requestContext) : base(logger, context, requestContext)
        { }

        public DirectMessage Add(DirectMessage directMessage)
        {
            if (directMessage.IsTransient())
            {
                return Context.DirectMessages
                    .Add(directMessage)
                    .Entity;
            }
            else
            {
                return directMessage;
            }

        }

        public async Task<DirectMessage> GetAsync(long id)
        {
            var directMessage = await Context.DirectMessages.AsQueryable()
                .Where(a => a.Id == id)
                .SingleOrDefaultAsync();
            return directMessage;
        }
    }
}
