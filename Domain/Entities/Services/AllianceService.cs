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

using DiscordBot.Domain.Entities.Alliances;
using DiscordBot.Domain.Seedwork;

namespace DiscordBot.Domain.Entities.Services
{
    public class AllianceService : Entity
    {
        public AllianceService()
        {
            
        }
        public AllianceService(Alliance alliance, Service service, AllianceServiceLevel allianceServiceLevel)
        {
            Alliance = alliance;
            _allianceId = alliance.Id;
            Service = service;
            _serviceId = service.Id;
            
            if (allianceServiceLevel == null) allianceServiceLevel = AllianceServiceLevel.Undefined;
            AllianceServiceLevel = allianceServiceLevel;
            _allianceServiceLevelId = allianceServiceLevel.Id;
        }
        
        public virtual Alliance Alliance { get; private set; }
        private long? _allianceId;
        
        public virtual Service Service { get; private set; }
        private long? _serviceId;
        
        public virtual AllianceServiceLevel AllianceServiceLevel { get; private set; }
        private int? _allianceServiceLevelId;
    }
}