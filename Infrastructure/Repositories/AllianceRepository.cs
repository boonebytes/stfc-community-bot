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
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using DiscordBot.Domain.Entities.Alliances;
using DiscordBot.Domain.Entities.Request;
using DiscordBot.Domain.Exceptions;
using DiscordBot.Domain.Seedwork;
using DiscordBot.Domain.Summaries;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Oracle.ManagedDataAccess.Client;

namespace DiscordBot.Infrastructure.Repositories
{
    public partial class AllianceRepository : BaseRepository, IAllianceRepository
    {
        public IUnitOfWork UnitOfWork => Context;

        public AllianceRepository(ILogger<AllianceRepository> logger, BotContext context, RequestContext requestContext) : base(logger, context, requestContext)
        { }

        public Alliance Add(Alliance alliance)
        {
            if (alliance.IsTransient())
            {
                if (Context.Alliances.Any(a => a.Acronym.ToUpper() == alliance.Acronym.ToUpper()
                                               || a.Name.ToUpper() == alliance.Name.ToUpper()))
                    throw new DuplicateNameException();
                return Context.Alliances
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
            var alliance = await Context.Alliances.AsQueryable()
                .Include(a => a.AssignedDiplomacy)
                    .ThenInclude(ad => ad.Related)
                .Where(a => a.Id == id)
                .SingleOrDefaultAsync();
            return alliance;
        }

        public async Task<Alliance> GetByNameOrAcronymAsync(string value)
        {
            var alliance = await Context.Alliances.AsQueryable()
                .Include(a => a.Group)
                .Include(a => a.Zones)
                .Where(a => a.Acronym.ToUpper() == value.ToUpper())
                .SingleOrDefaultAsync();

            if (alliance == null)
            {
                alliance = await Context.Alliances.AsQueryable()
                    .Where(a => a.Name.ToUpper() == value.ToUpper())
                    .SingleOrDefaultAsync();
            }
            return alliance;
        }

        public async Task<List<Alliance>> GetAllAsync()
        {
            return await Context.Alliances.ToListAsync();
        }

        public Alliance Update(Alliance alliance)
        {
            return Context.Alliances
                    .Update(alliance)
                    .Entity;
        }

        public List<Alliance> GetAllWithServers()
        {
            return Context.Alliances
                .AsQueryable()
                .Include(a => a.Zones)
                .Where(a => a.GuildId.HasValue)
                .Where(a => a.DefendSchedulePostChannel.HasValue)
                .ToList();
        }

        public List<AllianceGroup> GetAllianceGroups()
        {
            return Context.AllianceGroups
                .AsQueryable()
                .OrderBy(ag => ag.Name)
                .ToList();
        }

        public Alliance GetNextOnPostSchedule()
        {
            return Context.Alliances
                .AsQueryable()
                .Where(a => a.GuildId.HasValue)
                .Where(a => a.DefendSchedulePostChannel.HasValue)
                .ToList()
                .OrderBy(s => s.NextScheduledPost)
                .First();
        }

        public Alliance FindFromGuildId(ulong id)
        {
            var result = Context.Alliances
                .Include(a => a.Group)
                .Include(a => a.AssignedDiplomacy)
                    .ThenInclude(ad => ad.Related)
                .Where(a => a.GuildId == id);

            if (!result.Any())
                throw new BotDomainException("I'm not able to determine the alliance from this Discord server. Please contact support.");
            else
                return result.First();
        }
        
        public List<Alliance> GetTerritoryHelpersFromOwnerAlliance(long allianceId)
        {
            var results = new List<Alliance>();
            
            var alliance = Context.Alliances
                .Include(a => a.AssignedDiplomacy)
                    .ThenInclude(ad => ad.Related)
                .Include(a => a.AssignedDiplomacy)
                    .ThenInclude(ad => ad.Relationship)
                .Include(a => a.Group)
                    .ThenInclude(g => g.Alliances)
                .FirstOrDefault(a => a.Id == allianceId);


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
        
        public List<Alliance> GetTerritoryHelpersFromHelpingAlliance(long allianceId)
        {
            
            var results = new List<Alliance>();
            
            var alliance = Context.Alliances
                .Include(a => a.Group)
                .ThenInclude(g => g.Alliances)
                .FirstOrDefault(a => a.Id == allianceId);

            if (alliance == null) return results;
            
            if (alliance.Group != null && alliance.Group.Id != 0)
            {
                results.AddRange(alliance.Group.Alliances);
            }

            var allies = Context.Diplomacies
                .Include(d => d.Owner)
                    .ThenInclude(o => o.Group)
                .Include(d => d.Related)
                .Include(d => d.Relationship)
                .Where(d => d.Related == alliance && d.Relationship == DiplomaticRelation.Allied)
                .Select(d => d.Owner);

            foreach (var ally in allies)
            {
                if (!results.Contains(ally))
                {
                    results.Add(ally);
                }
            }

            return results;
        }

        public Alliance FlagSchedulePosted(Alliance alliance)
        {
            alliance.FlagPosted();
            return Update(alliance);
        }

        public async Task InitPostSchedule()
        {
            var alliances = Context.Alliances
                .AsQueryable()
                .Where(a =>
                    a.GuildId.HasValue
                    && a.DefendSchedulePostChannel.HasValue
                    && !string.IsNullOrEmpty(a.DefendSchedulePostTime)
                    && !a.NextScheduledPost.HasValue
                );
            foreach (var alliance in alliances)
            {
                alliance.SetNextScheduledPost();
            }
            await Context.SaveEntitiesAsync();
        }

        public async Task UpdateInventory(
            DateTime effectiveDate,
            decimal isogen1, decimal isogen2, decimal isogen3, decimal cores, decimal diodes, decimal emitters, decimal reactors,
            decimal reserves, decimal collisionalPlasma, decimal magneticPlasma, decimal superconductors
        )
        {
            /*
                in_Alliance_ID number,
                in_Effective_Date date,
                in_iso1 number,
                in_iso2 number,
                in_iso3 number,
                in_cores number,
                in_diodes number,
                in_emitters number,
                in_reactors number,
                in_collisional_plasma number,
                in_magnetic_plasma number,
                in_superconductors number,
                in_reserves number
             */
            var allianceId = RequestContext.GetAllianceId();
            
            var parameters = new DynamicParameters();
            parameters.Add(":in_Alliance_ID", allianceId.Value, DbType.Int64);
            parameters.Add(":in_Effective_Date", effectiveDate, DbType.Date);
            parameters.Add(":in_iso1", isogen1, DbType.Decimal);
            parameters.Add(":in_iso2", isogen2, DbType.Decimal);
            parameters.Add(":in_iso3", isogen3, DbType.Decimal);
            parameters.Add(":in_cores", cores, DbType.Decimal);
            parameters.Add(":in_diodes", diodes, DbType.Decimal);
            parameters.Add(":in_emitters", emitters, DbType.Decimal);
            parameters.Add(":in_reactors", reactors, DbType.Decimal);
            parameters.Add(":in_collisional_plasma", collisionalPlasma, DbType.Decimal);
            parameters.Add(":in_magnetic_plasma", magneticPlasma, DbType.Decimal);
            parameters.Add(":in_superconductors", superconductors, DbType.Decimal);
            parameters.Add(":in_reserves", reserves, DbType.Decimal);
            
            /*
            var parameters = new
            {
                in_Alliance_ID = allianceId,
                in_Effective_Date = effectiveDate,
                in_iso1 = isogen1,
                in_iso2 = isogen2,
                in_iso3 = isogen3,
                in_cores = cores,
                in_diodes = diodes,
                in_emitters = emitters,
                in_reactors = reactors,
                in_collisional_plasma = collisionalPlasma,
                in_magnetic_plasma = magneticPlasma,
                in_superconductors = superconductors,
                in_reserves = reserves
            };
            */
            
            var conn = Context.Database.GetDbConnection();
            //var cmd = new CommandDefinition("PKG_INVENTORY.INSERT_ALLIANCE_INVENTORY", parameters, commandType: CommandType.StoredProcedure);
            await conn.QueryAsync("CALL PKG_INVENTORY.INSERT_ALLIANCE_INVENTORY(:in_Alliance_ID, :in_Effective_Date, :in_iso1, :in_iso2, :in_iso3, :in_cores, :in_diodes, :in_emitters, :in_reactors, :in_collisional_plasma, :in_magnetic_plasma, :in_superconductors, :in_reserves)", parameters);
        }
        
        public Task<List<TerritoryInventory>> GetTerritoryInventory(Alliance alliance)
        {
            var conn = Context.Database.GetDbConnection();
            var results = conn.Query<TerritoryInventory>(SqlSelectTerritoryInventory, new { AllianceId = alliance.Id });
            return Task.FromResult(results.ToList());
        }

        public Task<List<StarbaseInventory>> GetStarbaseInventory(Alliance alliance)
        {
            var conn = Context.Database.GetDbConnection();
            var results = conn.Query<StarbaseInventory>(SqlSelectStarbaseInventory, new { AllianceId = alliance.Id });
            return Task.FromResult(results.ToList());
        }
    }
}
