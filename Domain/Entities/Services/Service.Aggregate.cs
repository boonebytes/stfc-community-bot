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