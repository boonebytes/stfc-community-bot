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
