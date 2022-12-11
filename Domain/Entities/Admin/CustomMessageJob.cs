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
using DiscordBot.Domain.Entities.Alliances;
using DiscordBot.Domain.Events;
using DiscordBot.Domain.Seedwork;

namespace DiscordBot.Domain.Entities.Admin;

public partial class CustomMessageJob : Entity
{
    protected CustomMessageJob()
    {
    }
    
    public virtual DateTime ScheduledTimestamp { get; private set; }
    public virtual ulong FromUser { get; private set; }
    public virtual string FromUsername { get; private set; }
    public virtual string FromUserNickname { get; private set; }
    
    private long? _allianceId;
    public virtual Alliance Alliance { get; private set; }
    
    public virtual ulong ChannelId { get; private set; }
    public virtual string Message { get; private set; }
    
    //private int _jobStatusId = JobStatus.Unspecified.Id;
    public virtual JobStatus Status { get; private set; }

    public void AddCustomMessageJobChangedDomainEvent()
    {
        this.AddDomainEvent(new CustomMessageJobUpdatedDomainEvent(this));
    }
}