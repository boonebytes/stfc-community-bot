using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DiscordBot.Domain.Entities.Alliances;
using DiscordBot.Domain.Exceptions;
using DiscordBot.Domain.Seedwork;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DiscordBot.Infrastructure.Repositories
{
    public class AllianceRepository : IAllianceRepository
    {
        private readonly ILogger<AllianceRepository> _logger;
        private readonly BotContext _context;

        public IUnitOfWork UnitOfWork
        {
            get
            {
                return _context;
            }
        }

        public AllianceRepository(ILogger<AllianceRepository> logger, BotContext context)
        {
            _logger = logger;
            _context = context;
        }

        public Alliance Add(Alliance alliance)
        {
            if (alliance.IsTransient())
            {
                return _context.Alliances
                    .Add(alliance)
                    .Entity;
            }
            else
            {
                return alliance;
            }

        }

        public async Task<Alliance> GetAsync(long id)
        {
            var alliance = await _context.Alliances
                .Where(z => z.Id == id)
                .SingleOrDefaultAsync();
            return alliance;
        }

        public async Task<List<Alliance>> GetAllAsync()
        {
            return await _context.Alliances.ToListAsync();
        }

        public Alliance Update(Alliance alliance)
        {
            return _context.Alliances
                    .Update(alliance)
                    .Entity;
        }

        public List<Alliance> GetAllWithServers()
        {
            return _context.Alliances
                .Where(a => a.GuildId.HasValue)
                .Where(a => a.DefendSchedulePostChannel.HasValue)
                .ToList();
        }

        public Alliance GetNextOnPostSchedule()
        {
            return _context.Alliances
                .Where(a => a.GuildId.HasValue)
                .Where(a => a.DefendSchedulePostChannel.HasValue)
                .ToList()
                .OrderBy(s => s.NextScheduledPost)
                .First();
        }

        public Alliance FindFromGuildId(ulong id)
        {
            var result = _context.Alliances
                .Include(a => a.Group)
                .Include(a => a.AssignedDiplomacy)
                    .ThenInclude(ad => ad.Related)
                .Where(a => a.GuildId == id);

            if (result == null || result.Count() == 0)
                throw new BotDomainException("I'm not able to determine the alliance from this Discord server. Please contact support.");
            else
                return result.First();
        }

        public Alliance FlagSchedulePosted(Alliance alliance)
        {
            alliance.FlagPosted();
            return Update(alliance);
        }

        public async Task InitPostSchedule()
        {
            var alliances = _context.Alliances
                .Where(a =>
                    a.GuildId.HasValue
                    && a.DefendSchedulePostChannel.HasValue
                    && !string.IsNullOrEmpty(a.DefendSchedulePostTime)
                    && !a.NextScheduledPost.HasValue
                );
            foreach (var alliance in alliances)
            {
                alliance.SetNextScheduledPost();
            }
            await _context.SaveEntitiesAsync();
        }
    }
}
