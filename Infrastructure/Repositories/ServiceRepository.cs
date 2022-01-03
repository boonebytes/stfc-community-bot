using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DiscordBot.Domain.Entities.Services;
using DiscordBot.Domain.Seedwork;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DiscordBot.Infrastructure.Repositories
{
    public class ServiceRepository : BaseRepository, IServiceRepository
    {
        public IUnitOfWork UnitOfWork { get; }
        
        public ServiceRepository(ILogger<ServiceRepository> logger, BotContext context) : base(logger, context)
        { }

        public Service Add(Service service)
        {
            if (service.IsTransient())
            {
                return _context.Services
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
            return _context.Services
                .Update(service)
                .Entity;
        }

        public async Task<Service> GetAsync(long id)
        {
            var service = await _context.Services
                .Include(s => s.Zone)
                .Include(s => s.Costs)
                    .ThenInclude(sc => sc.Resource)
                .SingleOrDefaultAsync(s => s.Id == id);
            return service;
        }

        public async Task<List<Service>> GetByZoneIdAsync(long id)
        {
            var services = await _context.Services
                .Include(s => s.Zone)
                .Include(s => s.Costs)
                    .ThenInclude(sc => sc.Resource)
                .Where(s => s.Zone.Id == id)
                .ToListAsync();
            return services;
        }

        public async Task<List<Service>> GetByZoneNameAsync(string name)
        {
            var zone = await _context.Zones
                .AsQueryable()
                .AsNoTracking()
                .SingleOrDefaultAsync(z => z.Name.ToUpper() == name.ToUpper());
            if (zone == null) return null;

            return await GetByZoneIdAsync(zone.Id);
        }
    }
}