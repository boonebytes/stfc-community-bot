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

namespace DiscordBot.Domain.Entities.Zones
{
    public partial class Zone : IAggregateRoot
    {
        public Zone(
                long id,
                string name,
                int level,
                Alliance owner,
                string defendUtcDayOfWeek,
                string defendUtcTime,
                string notes
            )
        {
            Id = id;
            this.Update(
                name,
                level,
                owner,
                defendUtcDayOfWeek,
                defendUtcTime,
                notes
                );
        }

        public void Update(
                string name,
                int level,
                Alliance owner,
                string defendUtcDayOfWeek,
                string defendUtcTime,
                string notes
            )
        {
            Name = name;
            if (level >= 1 && level <= 3)
                Level = level;

            Owner = owner;
            DefendUtcDayOfWeek = defendUtcDayOfWeek;
            DefendUtcTime = defendUtcTime;
            Notes = notes;

            AddZoneChangedDomainEvent();
        }
    }
}
