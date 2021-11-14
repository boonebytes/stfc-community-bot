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
    public class AllianceRepository : BaseRepository, IAllianceRepository
    {
        public IUnitOfWork UnitOfWork
        {
            get
            {
                return _context;
            }
        }

        public AllianceRepository(ILogger<AllianceRepository> logger, BotContext context) : base(logger, context)
        { }

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
            var alliance = await _context.Alliances.AsQueryable()
                .Where(a => a.Id == id)
                .SingleOrDefaultAsync();
            return alliance;
        }

        public async Task<Alliance> GetByNameOrAcronymAsync(string value)
        {
            var alliance = await _context.Alliances.AsQueryable()
                    .Where(a => a.Acronym.ToUpper() == value.ToUpper())
                    .SingleOrDefaultAsync();

            if (alliance == null || alliance == default)
            {
                alliance = await _context.Alliances.AsQueryable()
                    .Where(a => a.Name.ToUpper() == value.ToUpper())
                    .SingleOrDefaultAsync();
            }
            return alliance;
        }

        public async Task<List<Alliance>> GetAllAsync()
        {
            return await _context.Alliances.ToAsyncEnumerable().ToListAsync();
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
                .AsQueryable()
                .Where(a => a.GuildId.HasValue)
                .Where(a => a.DefendSchedulePostChannel.HasValue)
                .ToList();
        }

        public List<AllianceGroup> GetAllianceGroups()
        {
            return _context.AllianceGroups
                .AsQueryable()
                .OrderBy(ag => ag.Name)
                .ToList();
        }

        public Alliance GetNextOnPostSchedule()
        {
            return _context.Alliances
                .AsQueryable()
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
                .AsQueryable()
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
