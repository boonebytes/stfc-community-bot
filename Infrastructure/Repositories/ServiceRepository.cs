using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DiscordBot.Domain.Entities.Request;
using DiscordBot.Domain.Entities.Services;
using DiscordBot.Domain.Entities.Zones;
using DiscordBot.Domain.Seedwork;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Z.EntityFramework.Plus;

namespace DiscordBot.Infrastructure.Repositories
{
    public class ServiceRepository : BaseRepository, IServiceRepository
    {
        public IUnitOfWork UnitOfWork { get; }
        
        public ServiceRepository(ILogger<ServiceRepository> logger, BotContext context, RequestContext requestContext) : base(logger, context, requestContext)
        { }

        public Service Add(Service service)
        {
            if (service.IsTransient())
            {
                return Context.Services
                    .Add(service)
                    .Entity;
            }
            else
            {
                return service;
            }
        }

        public Service Update(Service service)
        {
            return Context.Services
                .Update(service)
                .Entity;
        }

        public async Task<Service> GetAsync(long id)
        {
            var service = await Context.Services
                .Include(s => s.Zone)
                .Include(s => s.Costs)
                    .ThenInclude(sc => sc.Resource)
                .SingleOrDefaultAsync(s => s.Id == id);
            return service;
        }

        public async Task<List<Service>> GetByZoneIdAsync(long id)
        {
            var services = await Context.Services
                .Include(s => s.Zone)
                .Include(s => s.Costs)
                    .ThenInclude(sc => sc.Resource)
                .Where(s => s.Zone.Id == id)
                .ToListAsync();
            return services;
        }

        public async Task<List<Service>> GetByZoneNameAsync(string name)
        {
            var zone = await Context.Zones
                .AsQueryable()
                .AsNoTracking()
                .SingleOrDefaultAsync(z => z.Name.ToUpper() == name.ToUpper());
            if (zone == null) return null;

            return await GetByZoneIdAsync(zone.Id);
        }

        public async Task<List<Service>> GetByAllianceIdAsync(long id, AllianceServiceLevel allianceServiceLevel = null)
        {
            if (allianceServiceLevel == null)
            {
                return await Context.Services
                    .Include(s => s.Zone)
                    .Include(s => s.AllianceServices)
                        .ThenInclude(allianceService => allianceService.Alliance)
                    .Include(s => s.AllianceServices)
                        .ThenInclude(allianceService => allianceService.AllianceServiceLevel)
                    .Include(s => s.Costs)
                        .ThenInclude(c => c.Resource)
                    .IncludeFilter(s => s.AllianceServices.Where(allianceServices => allianceServices.Alliance.Id == id))
                    .Where(s => s.Zone.Owner.Id == id)
                    .OrderBy(s => s.Zone.Name)
                    .ToListAsync();
            }
            return await Context.Services
                .Include(s => s.Zone)
                .Include(s => s.AllianceServices)
                    .ThenInclude(allianceService => allianceService.Alliance)
                .Include(s => s.AllianceServices)
                    .ThenInclude(allianceService => allianceService.AllianceServiceLevel)
                .Include(s => s.Costs)
                    .ThenInclude(c => c.Resource)
                .IncludeFilter(s => s.AllianceServices.Where(allianceServices => allianceServices.Alliance.Id == id && allianceServices.AllianceServiceLevel == allianceServiceLevel))
                .Where(s => s.Zone.Owner.Id == id)
                .OrderBy(s => s.Zone.Name)
                .ToListAsync();
        }

        public async Task<Dictionary<Resource, long>> GetCostByAllianceServiceLevelAsync(long id, AllianceServiceLevel allianceServiceLevel = null)
        {
            try
            {
                var results = Context.AllianceServices
                    .Include(allianceService => allianceService.Service.Costs)
                    .ThenInclude(c => c.Resource)
                    .Where(allianceService =>
                        allianceService.Alliance.Id == id
                        && allianceService.AllianceServiceLevel.Id == allianceServiceLevel.Id
                        && allianceService.Service.Zone.Owner.Id == id)
                    .SelectMany(allianceService => allianceService.Service.Costs)
                    .AsEnumerable();
                var groupedResults = results
                    .GroupBy(sc => sc.Resource)
                    .Select(i => new
                    {
                        Resource = i.Key,
                        TotalCost = i.Sum(sc => sc.Cost)
                    });
                return groupedResults.ToDictionary(i => i.Resource, i => i.TotalCost);
            }
            catch (Exception e)
            {
                Logger.LogError(e, "Error getting costs by alliance service level");
                return new Dictionary<Resource, long>();
            }
        }
    }
}