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
        public IUnitOfWork UnitOfWork => _context;

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
                .Include(z => z.ZoneNeighbours)
                    .ThenInclude(zn => zn.ToZone)
                        .ThenInclude(tz => tz.Owner)
                .SingleOrDefaultAsync(z => z.Id == id);
            return zone;
        }

        public async Task<Zone> GetByNameAsync(string name)
        {
            var zone = await _context.Zones
                .Include(z => z.Owner)
                .Include(z => z.ZoneNeighbours)
                    .ThenInclude(zn => zn.ToZone)
                        .ThenInclude(tz => tz.Owner)
                .Include(z => z.Services)
                .SingleOrDefaultAsync(z => z.Name.ToUpper() == name.ToUpper());
            return zone;
        }
        
        public List<Alliance> GetTerritoryHelpersFromZone(long zoneId)
        {
            var results = new List<Alliance>();

            var zone = _context.Zones
                .Include(z => z.Owner)
                .FirstOrDefault(z => z.Id == zoneId);

            if (zone.Owner == null) return results;
            
            var alliance = _context.Alliances
                .Include(a => a.AssignedDiplomacy)
                    .ThenInclude(ad => ad.Related)
                .Include(a => a.AssignedDiplomacy)
                    .ThenInclude(ad => ad.Relationship)
                .Include(a => a.Group)
                    .ThenInclude(g => g.Alliances)
                .FirstOrDefault(a => a.Id == zone.Owner.Id);


            if (alliance == null) return results;
            
            if (alliance.Group != null && alliance.Group.Id != 0)
            {
                results.AddRange(alliance.Group.Alliances);
            }

            var allies = alliance.AssignedDiplomacy
                .Where(ad => ad.Relationship == DiplomaticRelation.Allied)
                .Select(ad => ad.Related);

            foreach (var ally in allies)
            {
                if (!results.Contains(ally))
                {
                    results.Add(ally);
                }
            }

            return results;
        }
        
        public List<Alliance> GetPotentialHostiles(long id)
        {
            var thisZone = _context.Zones
                    .Include(z => z.Owner)
                        .ThenInclude(o => o.AssignedDiplomacy)
                            .ThenInclude(ad => ad.Related)
                                .ThenInclude(ada => ada.Zones)
                    .Include(z => z.ZoneNeighbours)
                        .ThenInclude(zn => zn.ToZone)
                            .ThenInclude(tz => tz.Owner)
                                .ThenInclude(tzo => tzo.Zones)
                    .SingleOrDefault(z => z.Id == id);

            if (thisZone == null) return default;
            /*
            var ownerHostiles = thisZone.Owner.AssignedDiplomacy
                                    .Where(ad =>
                                        ad.Relationship == DiplomaticRelation.Enemy
                                        && ad.Related.Zones.Count < 5
                                    );
            */

            List<Alliance> ownerFriendlies;
            List<Alliance> riskyNeighbours;
            if (thisZone.Owner == null)
            {
                ownerFriendlies = new();
                riskyNeighbours = thisZone.Neighbours.Where(n => n.Owner != null)
                    .Select(n => n.Owner)
                    .Distinct()
                    .Where(no =>
                            !ownerFriendlies.Contains(no)
                            // && no.Zones.Count < 5
                        )
                    .ToList();
            }
            else
            {
                ownerFriendlies = thisZone.Owner.AssignedDiplomacy
                    .Where(ad => ad.Relationship.Id >= DiplomaticRelation.Friendly.Id)
                    .Select(ad => ad.Related)
                    .ToList();

                riskyNeighbours = thisZone.Neighbours.Where(n => n.Owner != null)
                    .Select(n => n.Owner)
                    .Distinct()
                    .Where(no =>
                            !ownerFriendlies.Contains(no)
                            && no != thisZone.Owner
                            && (
                                thisZone.Owner.Group == null
                                || no.Group == null
                                || no.Group != thisZone.Owner.Group
                            )
                        // && no.Zones.Count < 5
                        )
                    .ToList();
            }
            

            return riskyNeighbours;
        }

        public async Task<List<Zone>> GetAllAsync(long? allianceId = null, bool withTracking = true)
        {
            QueryTrackingBehavior tracking = _context.ChangeTracker.QueryTrackingBehavior;
            if (!withTracking) tracking = QueryTrackingBehavior.NoTracking;

            List<long> interestedAlliances = GetInterestedAlliances(allianceId);
            return await _context.Zones
                            .Include(z => z.Owner)
                            .Include(z => z.ZoneNeighbours)
                                .ThenInclude(zn => zn.ToZone)
                                    .ThenInclude(tz => tz.Owner)
                            .AsTracking(tracking)
                            .Where(z => interestedAlliances.Contains(z.Owner.Id))
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
            var interestedAlliances = GetInterestedAlliances(allianceId);
            var next = _context.Zones
                            .Include(z => z.Owner)
                            .Include(z => z.ZoneNeighbours)
                                .ThenInclude(zn => zn.ToZone)
                                    .ThenInclude(tz => tz.Owner)
                            .Where(z => interestedAlliances.Contains(z.Owner.Id))
                            .OrderBy(z => z.NextDefend)
                            .ToList();
            return !next.Any() ? null : next.First();
        }

        public List<Zone> GetFromDayOfWeek(DayOfWeek dayOfWeek, long? allianceId = null)
        {
            var interestedAlliances = GetInterestedAlliances(allianceId);

            var results = _context.Zones
                .Include(z => z.Owner)
                .Include(z => z.ZoneNeighbours)
                    .ThenInclude(zn => zn.ToZone)
                        .ThenInclude(tz => tz.Owner)
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

            var interestedAlliances = GetInterestedAlliances(allianceId);

            var nextDefends = _context.Zones
                .Include(z => z.Owner)
                .Include(z => z.ZoneNeighbours)
                    .ThenInclude(zn => zn.ToZone)
                        .ThenInclude(tz => tz.Owner)
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

        public async Task InitZones(bool softUpdate = false)
        {
            var allZones = _context.Zones
                    .ToList();
            if (softUpdate)
                allZones = allZones.Where(z =>
                    !z.NextDefend.HasValue || z.NextDefend.Value < DateTime.UtcNow)
                    .ToList();
            foreach (var zone in allZones)
            {
                var potentialHostiles = GetPotentialHostiles(zone.Id)
                    .Select(h => h.Acronym)
                    .Distinct()
                    .OrderBy(h => h)
                    .ToList();
                if (potentialHostiles.Any())
                {
                    zone.SetThreats(string.Join(", ", potentialHostiles));
                }
                else
                {
                    zone.SetThreats(string.Empty);
                }
                zone.SetNextDefend(true);
            }
            await _context.SaveEntitiesAsync();
        }
    }
}
