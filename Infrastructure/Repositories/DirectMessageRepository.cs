using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DiscordBot.Domain.Entities.Admin;
using DiscordBot.Domain.Exceptions;
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
                return _context;
            }
        }

        public DirectMessageRepository(ILogger<DirectMessageRepository> logger, BotContext context) : base(logger, context)
        { }

        public DirectMessage Add(DirectMessage directMessage)
        {
            if (directMessage.IsTransient())
            {
                return _context.DirectMessages
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
            var directMessage = await _context.DirectMessages.AsQueryable()
                .Where(a => a.Id == id)
                .SingleOrDefaultAsync();
            return directMessage;
        }
    }
}
