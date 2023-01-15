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
using DiscordBot.Domain.Entities.Alliances;
using DiscordBot.Domain.Entities.Request;
using DiscordBot.Domain.Exceptions;
using DiscordBot.Domain.Seedwork;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DiscordBot.Infrastructure.Repositories
{
    public class ReactMessageRepository : BaseRepository, IReactMessageRepository
    {
        public IUnitOfWork UnitOfWork => Context;

        public ReactMessageRepository(ILogger<ReactMessageRepository> logger, BotContext context, RequestContext requestContext) : base(logger, context, requestContext)
        { }

        public ReactMessage Add(ReactMessage message)
        {
            if (message.IsTransient())
            {
                return Context.ReactMessages
                    .Add(message)
                    .Entity;
            }
            else
            {
                return message;
            }

        }

        public async Task<ReactMessage> GetAsync(long id)
        {
            var message = await Context.ReactMessages.AsQueryable()
                .Where(m => m.Id == id)
                .FirstOrDefaultAsync();
            return message;
        }

        public async Task<ReactMessage> GetByDiscordMessageIdAsync(ulong discordMessageId)
        {
            var message = await Context.ReactMessages.AsQueryable()
                .Where(m => m.MessageId == discordMessageId)
                .FirstOrDefaultAsync();
            return message;
        }

        public ReactMessage Update(ReactMessage message)
        {
            return Context.ReactMessages
                    .Update(message)
                    .Entity;
        }
    }
}
