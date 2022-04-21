using System.Collections.Generic;
using System.Linq;
using DiscordBot.Domain.Entities.Zones;
using DiscordBot.Domain.Seedwork;

namespace DiscordBot.Domain.Entities.Services
{
    public partial class Service : Entity
    {
        public Service()
        {
            _costs = new List<ServiceCost>();
        }
        public virtual string Name { get; private set; }
        public virtual string Description { get; private set; }

        private long? _zoneId;
        public virtual Zone Zone { get; private set; }

        private readonly List<ServiceCost> _costs;
        public IReadOnlyCollection<ServiceCost> Costs => _costs;

        private readonly List<AllianceService> _allianceServices;
        public IReadOnlyCollection<AllianceService> AllianceServices => _allianceServices;
        
        public void SetCost(Resource resource, long cost)
        {
            var existingCost = _costs.FirstOrDefault(c => c.Resource == resource);
            if (existingCost == null)
            {
                if (cost > 0)
                {
                    var newRecord = new ServiceCost(resource, cost, this);
                    _costs.Add(newRecord);
                }
            }
            else
            {
                if (cost > 0)
                {
                    existingCost.SetCost(cost);
                }
                else
                {
                    _costs.Remove(existingCost);
                }
            }
        }
    }
}