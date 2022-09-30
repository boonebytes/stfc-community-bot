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

namespace DiscordBot.Domain.Entities.Alliances
{
    public partial class Alliance : IAggregateRoot
    {
        public Alliance(
                long id,
                string name,
                string acronym,
                AllianceGroup group,
                ulong? guildId,
                ulong? defendSchedulePostChannelId,
                string defendSchedulePostTime,
                int? defendBroadcastLeadTime
            )
        {
            Id = id;
            this.Update(
                name,
                acronym,
                group,
                guildId,
                defendSchedulePostChannelId,
                defendSchedulePostTime,
                defendBroadcastLeadTime
                );
        }
        
        public void Update(
                string name,
                string acronym,
                AllianceGroup group,
                ulong? guildId,
                ulong? defendSchedulePostChannelId,
                string defendSchedulePostTime,
                int? defendBroadcastLeadTime
            )
        {
            Name = name;
            Acronym = acronym;
            Group = group;
            GuildId = guildId;
            DefendSchedulePostChannel = defendSchedulePostChannelId;
            DefendSchedulePostTime = defendSchedulePostTime;
            DefendBroadcastLeadTime = defendBroadcastLeadTime;

            this.AddAllianceChangedDomainEvent();
        }

    }
}
