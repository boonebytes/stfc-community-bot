/*
Copyright 2022 Boonebytes

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/

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
