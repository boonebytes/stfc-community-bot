using System.Collections.Generic;
using System.Runtime.CompilerServices;
using DiscordBot.Domain.Seedwork;

namespace DiscordBot.Domain.Entities.Zones
{
    public class ServiceCost : Entity
    {
        public ServiceCost()
        {
            
        }
        public ServiceCost(Resource resource, long cost, Service service)
        {
            Resource = resource;
            Cost = cost;
            Service = service;
            _serviceId = service.Id;
        }
        
        public virtual Resource Resource { get; private set; }
        public virtual long Cost { get; private set; }

        private long? _serviceId;
        public virtual Service Service { get; private set; }

        public void SetCost(long cost)
        {
            Cost = cost;
        }
    }
}