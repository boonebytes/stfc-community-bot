using System.Collections.Generic;
using DiscordBot.Domain.Entities.Zones;
using DiscordBot.Domain.Seedwork;

namespace DiscordBot.Domain.Entities.Services
{
    public partial class Service : IAggregateRoot
    {
        public Service(
                long id,
                string name,
                string description,
                Zone zone
            )
        {
            _costs = new List<ServiceCost>();
            
            this.Id = id;
            this.Zone = zone;
            this._zoneId = zone.Id;
            this.Update(name, description);
        }

        public void Update(
                string name,
                string description
            )
        {
            this.Name = name;
            this.Description = description;
        }
    }
}