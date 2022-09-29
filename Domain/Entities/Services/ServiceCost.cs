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

using DiscordBot.Domain.Entities.Zones;
using DiscordBot.Domain.Seedwork;

namespace DiscordBot.Domain.Entities.Services
{
    public class ServiceCost : Entity
    {
        public ServiceCost()
        {
            
        }
        public ServiceCost(Resource resource, long cost, Service service)
        {
            Resource = resource;
            _resourceId = resource.Id;
            Cost = cost;
            Service = service;
            _serviceId = service.Id;
        }
        
        public virtual Resource Resource { get; private set; }
        private int? _resourceId;
        public virtual long Cost { get; private set; }

        private long? _serviceId;
        public virtual Service Service { get; private set; }

        public void SetCost(long cost)
        {
            Cost = cost;
        }
    }
}