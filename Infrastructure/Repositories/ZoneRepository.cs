using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DiscordBot.Domain.Entities.Alliances;
using DiscordBot.Domain.Entities.Request;
using DiscordBot.Domain.Entities.Zones;
using DiscordBot.Domain.Seedwork;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DiscordBot.Infrastructure.Repositories
{
    public class ZoneRepository : BaseRepository, IZoneRepository
    {
        public IUnitOfWork UnitOfWork => Context;

        public ZoneRepository(ILogger<ZoneRepository> logger, BotContext context, RequestContext requestContext) : base(logger, context, requestContext)
        { }

        public Zone Add(Zone zone)
        {
            if (zone.IsTransient())
            {
                return Context.Zones
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
            var povAllianceId = RequestContext.GetAllianceId();
            
            var zone = await Context.Zones
                .Include(z => z.Owner)
                .Include(z => z.ZoneNeighbours)
                    .ThenInclude(zn => zn.ToZone)
                        .ThenInclude(tz => tz.Owner)
                .SingleOrDefaultAsync(z => z.Id == id);

            zone.Threats = String.Join( ", ", GetContenders(zone.Id).Select(a => a.Acronym));
            return zone;
        }

        public async Task<Zone> GetByNameAsync(string name)
        {
            var povAllianceId = RequestContext.GetAllianceId();
        
            var zone = await Context.Zones
                .Include(z => z.Owner)
                .Include(z => z.ZoneNeighbours)
                    .ThenInclude(zn => zn.ToZone)
                        .ThenInclude(tz => tz.Owner)
                .Include(z => z.Services)
                .SingleOrDefaultAsync(z => z.Name.ToUpper() == name.ToUpper());

            zone.Threats = String.Join( ", ", GetContenders(zone.Id).Select(a => a.Acronym));
            return zone;
        }
        
        public List<Alliance> GetTerritoryHelpersFromZone(long zoneId)
        {
            var results = new List<Alliance>();

            var zone = Context.Zones
                .Include(z => z.Owner)
                .FirstOrDefault(z => z.Id == zoneId);

            if (zone.Owner == null) return results;
            
            var alliance = Context.Alliances
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
        
        public List<Alliance> GetContenders(long zoneId)
        {
            var povAllianceId = RequestContext.GetAllianceId();
            
            var thisZone = Context.Zones
                .Include(z => z.Owner)
                    .ThenInclude(o => o.AssignedDiplomacy)
                        .ThenInclude(ad => ad.Related)
                .Include(z => z.ZoneNeighbours)
                    .ThenInclude(zn => zn.ToZone)
                        .ThenInclude(tz => tz.Owner)
                .SingleOrDefault(z => z.Id == zoneId);
            
            if (thisZone == null) return null;
            
            var thisZoneOwner = thisZone.Owner;
            var zoneNeighbours = thisZone.ZoneNeighbours.Select(zn => zn.ToZone);

            var povAlliance = Context.Alliances
                .FirstOrDefault(a => a.Id == povAllianceId);

            /*
            var ownerHostiles = thisZone.Owner.AssignedDiplomacy
                                    .Where(ad =>
                                        ad.Relationship == DiplomaticRelation.Enemy
                                        && ad.Related.Zones.Count < 5
                                    );
            */

            List<Alliance> ownerFriendlies = new();
            List<Alliance> riskyNeighbours;
            if (thisZone.Owner == null)
            {
                riskyNeighbours = thisZone.Neighbours.Where(n => n.Owner != null)
                    .Select(n => n.Owner)
                    .Distinct()
                    //.Where(no => no.Zones.Count < 5)
                    .ToList();
            }
            else
            {
                bool hideFriendliesFromContenders =
                    thisZoneOwner != null
                    && povAlliance != null
                    && (
                        thisZoneOwner.Id == povAlliance.Id
                        || (thisZoneOwner.Group != null && povAlliance.Group != null && thisZoneOwner.Group == povAlliance.Group)
                        || thisZoneOwner.AssignedDiplomacy.Any(ad =>
                            ad.Related == povAlliance
                            && (
                                ad.Relationship == DiplomaticRelation.Friendly
                                || ad.Relationship == DiplomaticRelation.Allied
                            )
                        )
                    );
                
                if (hideFriendliesFromContenders)
                {
                    ownerFriendlies = thisZone.Owner.AssignedDiplomacy
                        .Where(ad => ad.Relationship.Id >= DiplomaticRelation.Friendly.Id)
                        .Select(ad => ad.Related)
                        .ToList();
                }

                riskyNeighbours = thisZone.Neighbours.Where(n => n.Owner != null)
                    .Select(n => n.Owner)
                    .Distinct()
                    .Where(no =>
                            no != thisZone.Owner
                            && (
                                !hideFriendliesFromContenders
                                || (
                                    !ownerFriendlies.Contains(no)
                                    && (
                                        thisZone.Owner.Group == null
                                        || no.Group == null
                                        || no.Group != thisZone.Owner.Group
                                    )
                                )
                            )
                        // && no.Zones.Count < 5
                    )
                    .ToList();
            }
            

            return riskyNeighbours;
        }

        public async Task<List<Zone>> GetAllAsync(long? allianceId = null, bool withTracking = true)
        {
            QueryTrackingBehavior tracking = Context.ChangeTracker.QueryTrackingBehavior;
            if (!withTracking) tracking = QueryTrackingBehavior.NoTracking;

            List<long> interestedAlliances = GetInterestedAlliances(allianceId);
            var results = await Context.Zones
                            .Include(z => z.Owner)
                            .Include(z => z.ZoneNeighbours)
                                .ThenInclude(zn => zn.ToZone)
                                    .ThenInclude(tz => tz.Owner)
                            .AsTracking(tracking)
                            .Where(z => interestedAlliances.Contains(z.Owner.Id))
                            .OrderBy(z => z.DefendEasternDay)
                            .ThenBy(z => z.DefendEasternTime)
                            .ToListAsync();

            foreach (var zone in results)
            {
                zone.Threats = String.Join(", ", GetContenders(zone.Id).Select(a => a.Acronym));
            }
            return results;
        }

        public async Task<List<Zone>> GetLookupListAsync()
        {
            return await Context.Zones
                .Include(z => z.Owner)
                .OrderBy(z => z.Name)
                .ToListAsync();
        }

        public Zone Update(Zone zone)
        {
            return Context.Zones
                    .Update(zone)
                    .Entity;
        }

        public Zone GetNextDefend(long? allianceId = null)
        {
            var interestedAlliances = GetInterestedAlliances(allianceId);
            var next = Context.Zones
                            .Include(z => z.Owner)
                            .Include(z => z.ZoneNeighbours)
                                .ThenInclude(zn => zn.ToZone)
                                    .ThenInclude(tz => tz.Owner)
                            .Where(z => interestedAlliances.Contains(z.Owner.Id))
                            .OrderBy(z => z.NextDefend)
                            .ToList();
            var result = !next.Any() ? null : next.First();

            if (result != null)
            {
                result.Threats = String.Join(", ", GetContenders(result.Id).Select(a => a.Acronym));
            }
            
            return result;
        }

        public List<Zone> GetFromDayOfWeek(DayOfWeek dayOfWeek, long? allianceId = null)
        {
            var interestedAlliances = GetInterestedAlliances(allianceId);

            var results = Context.Zones
                .AsSplitQuery()
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
            
            foreach (var zone in results)
            {
                zone.Threats = String.Join(", ", GetContenders(zone.Id).Select(a => a.Acronym));
            }
            
            return results;
        }


        public List<Zone> GetNext24Hours(DateTime? fromDate = null, long? allianceId = null)
        {
            if (!allianceId.HasValue) allianceId = RequestContext.GetAllianceId();

            if (!fromDate.HasValue)
            {
                fromDate = DateTime.UtcNow;
            }

            var interestedAlliances = GetInterestedAlliances(allianceId);

            var nextDefends = Context.Zones
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
            
            foreach (var zone in nextDefends)
            {
                zone.Threats = String.Join(", ", GetContenders(zone.Id).Select(a => a.Acronym));
            }
            
            return nextDefends;
        }

        public async Task InitZones(bool softUpdate = false)
        {
            var allZones = Context.Zones
                    .ToList();
            if (softUpdate)
                allZones = allZones.Where(z =>
                    !z.NextDefend.HasValue || z.NextDefend.Value < DateTime.UtcNow)
                    .ToList();
            foreach (var zone in allZones)
            {
                /*
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
                */
                zone.SetNextDefend(true);
            }
            await Context.SaveEntitiesAsync();
        }
    }
}
