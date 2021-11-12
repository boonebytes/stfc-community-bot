using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DiscordBot.Domain.Entities.Alliances;
using DiscordBot.Domain.Entities.Zones;
using DiscordBot.Domain.Seedwork;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DiscordBot.Infrastructure.Repositories
{
    public class ZoneRepository : BaseRepository, IZoneRepository
    {
        public IUnitOfWork UnitOfWork
        {
            get
            {
                return _context;
            }
        }

        public ZoneRepository(ILogger<ZoneRepository> logger, BotContext context) : base(logger, context)
        { }

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

        public async Task<List<Zone>> GetAllAsync(long? allianceId = null, bool withTracking = true)
        {
            QueryTrackingBehavior tracking = _context.ChangeTracker.QueryTrackingBehavior;
            if (!withTracking) tracking = QueryTrackingBehavior.NoTracking;

            List<long> interestedAlliances = GetInterestedAlliances(allianceId);
            return await _context.Zones
                            .Include(z => z.Owner)
                            .AsTracking(tracking)
                            .Where(z => interestedAlliances.Contains(z.Owner.Id))
                            //.OrderBy(z => z.NextDefend)
                            .OrderBy(z => z.DefendEasternDay)
                            .ThenBy(z => z.DefendEasternTime)
                            .ToListAsync();
        }

        public Zone Update(Zone zone)
        {
            return _context.Zones
                    .Update(zone)
                    .Entity;
        }

        public Zone GetNextDefend(long? allianceId = null)
        {
            List<long> interestedAlliances = GetInterestedAlliances(allianceId);
            var next = _context.Zones
                            .Include(z => z.Owner)
                            .Where(z => interestedAlliances.Contains(z.Owner.Id))
                            .OrderBy(z => z.NextDefend)
                            .ToList();
            if (next == null)
                return null;
            else
                return next.First();
        }

        public List<Zone> GetFromDayOfWeek(DayOfWeek dayOfWeek, long? allianceId = null)
        {
            List<long> interestedAlliances = GetInterestedAlliances(allianceId);

            var results = _context.Zones
                .Include(z => z.Owner)
                .Where(z => interestedAlliances.Contains(z.Owner.Id))
                .ToList()
                .Where(z =>
                        z.DefendEasternDay == dayOfWeek
                    )
                .OrderBy(z => z.DefendEasternTime)
                .ToList();
            return results;
        }


        public List<Zone> GetNext24Hours(DateTime? fromDate = null, long? allianceId = null)
        {
            if (!fromDate.HasValue)
            {
                fromDate = DateTime.UtcNow;
            }

            List<long> interestedAlliances = GetInterestedAlliances(allianceId);

            var nextDefends = _context.Zones
                .Include(z => z.Owner)
                .Where(z => interestedAlliances.Contains(z.Owner.Id))
                .ToList()
                .Where(z =>
                        z.NextDefend > fromDate.Value &&
                        z.NextDefend < fromDate.Value.AddDays(1)
                    )
                .OrderBy(z => z.NextDefend)
                .ToList();
            return nextDefends;
        }

        public async Task InitZones()
        {
            var allZones = _context.Zones.AsAsyncEnumerable();
                            //.Where(z => !z.NextDefend.HasValue || z.NextDefend.Value < DateTime.UtcNow);
            await allZones.ForEachAsync(z =>
                    z.SetNextDefend(true)
                );
            await _context.SaveEntitiesAsync();
        }
    }
}
