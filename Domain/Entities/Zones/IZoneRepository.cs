using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DiscordBot.Domain.Entities.Alliances;
using DiscordBot.Domain.Seedwork;

namespace DiscordBot.Domain.Entities.Zones
{
    public interface IZoneRepository : IRepository<Zone>
    {
        Zone Add(Zone zone);

        Zone Update(Zone zone);

        Task<Zone> GetAsync(long id);

        Task<Zone> GetByNameAsync(string id);

        List<Alliance> GetTerritoryHelpersFromZone(long zoneId);
        
        List<Alliance> GetContenders(long zoneId);

        Task<List<Zone>> GetAllAsync(long? allianceId = null, bool withTracking = true);

        Task<List<Zone>> GetLookupListAsync();

        Zone GetNextDefend(long? allianceId = null);

        List<Zone> GetFromDayOfWeek(DayOfWeek dayOfWeek, long? allianceId = null);

        List<Zone> GetNext24Hours(DateTime? fromDate = null, long? allianceId = null);

        Task InitZones(bool softUpdate = false);
    }
}
