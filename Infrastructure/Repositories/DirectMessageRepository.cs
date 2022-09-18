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
