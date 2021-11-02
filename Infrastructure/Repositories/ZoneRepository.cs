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

        public List<Zone> GetNext24Hours(DateTime? fromDate = null, long? allianceId = null)
        {
            if (!fromDate.HasValue)
            {
                fromDate = DateTime.UtcNow;
            }

            List<long> interestedAlliances;
            if (allianceId.HasValue)
            {
                var thisAlliance = _context.Alliances
                    .Include(a => a.Group)
                        .ThenInclude(ag => ag.Alliances)
                    .Include(a => a.AssignedDiplomacy)
                        .ThenInclude(ad => ad.Related)
                    .Where(a => a.Id == allianceId)
                    .FirstOrDefault();


                List<long> thisAllianceGroupMembers;

                if (thisAlliance.Group == null)
                {
                    thisAllianceGroupMembers = new();
                }
                else
                {
                    thisAllianceGroupMembers = thisAlliance.Group.Alliances
                                                .Select(gm => gm.Id).ToList();
                }

                var friendlies = _context.Diplomacies
                                .Where(d =>
                                        d.Owner == thisAlliance
                                        && (
                                            d.Relationship == DiplomaticRelation.Friendly
                                            || d.Relationship == DiplomaticRelation.Allied
                                        )
                                    );

                interestedAlliances = friendlies
                                            .Select(ag => ag.Related.Id).ToList()
                                            .Union(thisAllianceGroupMembers).ToList();
            }
            else
            {
                interestedAlliances = _context.Alliances
                                            .Select(a => a.Id)
                                            .ToList();
            }

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
            var allZones = _context.Zones; //.Where(z => !z.NextDefend.HasValue || z.NextDefend.Value < DateTime.UtcNow);
            await allZones.ForEachAsync(z =>
                    z.SetNextDefend()
                );
            await _context.SaveEntitiesAsync();
        }
    }
}
