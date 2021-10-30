﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DiscordBot.Domain.Entities.Alliances;
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

        public Alliance FlagSchedulePosted(Alliance alliance)
        {
            alliance.FlagPosted();
            return Update(alliance);
        }
    }
}
