using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DiscordBot.Domain.Seedwork;

namespace DiscordBot.Domain.Entities.Zones
{
    public interface IZoneRepository : IRepository<Zone>
    {
        Zone Add(Zone zone);

        Zone Update(Zone zone);

        Task<Zone> GetAsync(long id);

        Task<List<Zone>> GetAllAsync();

        List<Zone> GetNext24Hours(DateTime? fromDate = null, long? allianceId = null);

        Task InitZones();
    }
}
