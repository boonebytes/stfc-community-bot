using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DiscordBot.Domain.Entities.Zones;
using DiscordBot.Domain.Seedwork;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DiscordBot.Infrastructure.Repositories
{
    public class ZoneRepository : IZoneRepository
    {
        private readonly ILogger<ZoneRepository> _logger;
        private readonly BotContext _context;

        public IUnitOfWork UnitOfWork
        {
            get
            {
                return _context;
            }
        }

        public ZoneRepository(ILogger<ZoneRepository> logger, BotContext context)
        {
            _logger = logger;
            _context = context;
        }

        public Zone Add(Zone zone)
        {
            if (zone.IsTransient())
            {
                return _context.Zones
                    .Add(zone)
                    .Entity;
            }
            else
            {
                return zone;
            }
            
        }

        public async Task<Zone> GetAsync(long id)
        {
            var zone = await _context.Zones
                .Include(z => z.Owner)
                .Where(z => z.Id == id)
                .SingleOrDefaultAsync();
            return zone;
        }

        public async Task<List<Zone>> GetAllAsync()
        {
            return await _context.Zones
                .Include(z => z.Owner)
                .ToListAsync();
        }

        public Zone Update(Zone zone)
        {
            return _context.Zones
                    .Update(zone)
                    .Entity;
        }

        public List<Zone> GetNext24Hours(DateTime? fromDate = null)
        {
            if (!fromDate.HasValue)
            {
                fromDate = DateTime.UtcNow;
            }

            var nextDefends = _context.Zones
                .Include(z => z.Owner)
                .ToList()
                .Where(z =>
                        z.NextDefend > fromDate.Value &&
                        z.NextDefend < fromDate.Value.AddDays(1)
                    )
                .OrderBy(z => z.NextDefend)
                .ToList();
            return nextDefends;
        }
    }
}
