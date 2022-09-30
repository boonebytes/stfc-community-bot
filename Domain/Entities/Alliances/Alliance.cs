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

using System;
using System.Collections.Generic;
using System.Globalization;
using DiscordBot.Domain.Entities.Services;
using DiscordBot.Domain.Entities.Zones;
using DiscordBot.Domain.Events;
using DiscordBot.Domain.Seedwork;

namespace DiscordBot.Domain.Entities.Alliances
{
    public partial class Alliance : Entity
    {
        public Alliance()
        {
        }

        public virtual string Name { get; private set; }
        public virtual string Acronym { get; private set; }

        private long? _allianceGroupId;
        public virtual AllianceGroup Group { get; private set; }

        private readonly List<Diplomacy> _assignedDiplomacy;
        public IReadOnlyCollection<Diplomacy> AssignedDiplomacy => _assignedDiplomacy;

        private readonly List<Diplomacy> _receivedDiplomacy;
        public IReadOnlyCollection<Diplomacy> ReceivedDiplomacy => _receivedDiplomacy;

        public virtual ulong? GuildId { get; protected set; }
        public virtual ulong? DefendSchedulePostChannel { get; protected set; }
        public virtual string DefendSchedulePostTime { get; protected set; }
        public virtual int? DefendBroadcastLeadTime { get; protected set; }
        public virtual ulong? DefendBroadcastPingRole { get; protected set; }
        public virtual bool? DefendBroadcastPingForLowRisk { get; protected set; }
        public virtual ulong? AlliedBroadcastRole { get; protected set; }

        public void SetDefendBroadcastLeadTime(int? value)
        {
            DefendBroadcastLeadTime = value;
            AddAllianceChangedDomainEvent();
        }

        public void SetDefendBroadcastPingRole(ulong? value)
        {
            DefendBroadcastPingRole = value;
            AddAllianceChangedDomainEvent();
        }
        
        public void SetDefendBroadcastPingForLowRisk(bool? value)
        {
            DefendBroadcastPingForLowRisk = value;
            AddAllianceChangedDomainEvent();
        }

        public void SetAlliedBroadcastRole(ulong? value)
        {
            AlliedBroadcastRole = value;
            AddAllianceChangedDomainEvent();
        }

        public void SetDefendSchedulePostChannel(ulong? value)
        {
            if (value == DefendSchedulePostChannel) return;
            
            AddAllianceChangedDomainEvent();
            DefendSchedulePostChannel = value;
            if (!value.HasValue)
            {
                DefendSchedulePostTime = "";
                NextScheduledPost = null;
            }
            else if (DefendSchedulePostTime == "")
            {
                DefendSchedulePostTime = "8:00 AM";
                SetNextScheduledPost();
            }
        }
        
        private readonly List<Zone> _zones;
        public IReadOnlyCollection<Zone> Zones => _zones;
        
        private readonly List<AllianceService> _allianceServices;
        public IReadOnlyCollection<AllianceService> AllianceServices => _allianceServices;

        public virtual DateTime? NextScheduledPost { get; private set; }

        public void SetNextScheduledPost()
        {
            if (GuildId.HasValue && DefendSchedulePostChannel.HasValue && !string.IsNullOrEmpty(DefendSchedulePostTime))
            {
                CultureInfo culture = new CultureInfo("en-US");
                NextScheduledPost = DateTime.ParseExact(DefendSchedulePostTime, "h:mm tt", culture, DateTimeStyles.AssumeUniversal);
                if (NextScheduledPost.Value.ToUniversalTime() < DateTime.UtcNow) NextScheduledPost = NextScheduledPost.Value.AddDays(1);
                NextScheduledPost = NextScheduledPost.Value.ToUniversalTime();
            }
            else
            {
                NextScheduledPost = null;
            }
        }

        public void FlagPosted()
        {
            SetNextScheduledPost();
        }
        
        public void AddAllianceChangedDomainEvent()
        {
            this.AddDomainEvent(new AllianceUpdatedDomainEvent(this));
        }
    }
}
