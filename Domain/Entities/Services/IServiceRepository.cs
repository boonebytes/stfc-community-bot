using System.Collections.Generic;
using System.Threading.Tasks;
using DiscordBot.Domain.Seedwork;

namespace DiscordBot.Domain.Entities.Services
{
    public interface IServiceRepository  : IRepository<Service>
    {
        Service Add(Service service);

        Service Update(Service service);
        
        Task<Service> GetAsync(long id);

        Task<List<Service>> GetByZoneIdAsync(long id);

        Task<List<Service>> GetByZoneNameAsync(string name);

        Task<List<Service>> GetByAllianceIdAsync(long id);
    }
}