using System;
using DiscordBot.Domain.Seedwork;

namespace DiscordBot.Domain.Entities.Zones
{
    public class StarSystem : Entity
    {
        public StarSystem()
        {
        }

        public virtual string Name { get; private set; }

        private long? _zoneId;
        public virtual Zone Zone { get; private set; }

        private int? _resourceId;
        public Resource Resource
        {
            get
            {
                if (_resourceId.HasValue)
                    return Resource.From(_resourceId.Value);
                else
                    return null;
            }
        }

        public void SetResourceId(int? id)
        {
            _resourceId = id;
        }
    }
}
