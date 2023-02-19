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
using DiscordBot.Domain.Seedwork;
using DiscordBot.Domain.Summaries;

namespace DiscordBot.Domain.Entities.Alliances
{
    public interface IAllianceRepository : IRepository<Alliance>
    {
        Alliance Add(Alliance alliance);

        Alliance Update(Alliance alliance);

        Task<Alliance> GetAsync(long id);

        Task<Alliance> GetByNameOrAcronymAsync(string value);

        Task<List<Alliance>> GetAllAsync();

        List<Alliance> GetAllWithServers();

        List<AllianceGroup> GetAllianceGroups();

        Alliance GetNextOnPostSchedule();

        Alliance FindFromGuildId(ulong id);

        List<Alliance> GetTerritoryHelpersFromOwnerAlliance(long allianceId);
        List<Alliance> GetTerritoryHelpersFromHelpingAlliance(long allianceId);

        Alliance FlagSchedulePosted(Alliance alliance);

        Task InitPostSchedule();

        Task<List<TerritoryInventory>> GetTerritoryInventory(Alliance alliance);
        
        Task<List<StarbaseInventory>> GetStarbaseInventory(Alliance alliance);

        Task UpdateInventory(
            DateTime effectiveDate,
            decimal isogen1, decimal isogen2, decimal isogen3, decimal cores, decimal diodes, decimal emitters, decimal reactors,
            decimal reserves, decimal collisionalPlasma, decimal magneticPlasma, decimal superconductors
        );
    }
}
