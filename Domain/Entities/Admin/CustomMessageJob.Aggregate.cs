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
using DiscordBot.Domain.Seedwork;

namespace DiscordBot.Domain.Entities.Admin;

public  partial class CustomMessageJob : IAggregateRoot
{
    public CustomMessageJob(
        DateTime scheduledTmestamp,
        ulong fromUser,
        string fromUsername,
        string fromUserNickname,
        Alliance alliance,
        ulong channelId,
        string message)
    {
        ScheduledTimestamp = scheduledTmestamp;
        FromUser = fromUser;
        FromUsername = fromUsername;
        FromUserNickname = fromUserNickname;
        SetAlliance(alliance);
        ChannelId = channelId;
        Message = message;
        SetStatus(JobStatus.Unspecified);
    }

    protected void SetAlliance(Alliance alliance)
    {
        Alliance = alliance;
        _allianceId = alliance.Id;
    }

    protected void SetStatus(JobStatus status)
    {
        Status = status;
        AddCustomMessageJobChangedDomainEvent();
    }

    public void Schedule()
    {
        if (ScheduledTimestamp > DateTime.UtcNow)
        {
            SetStatus(JobStatus.Scheduled);
        }
        else
        {
            throw new InvalidOperationException("Cannot schedule; timestamp is in the past");
        }
    }

    public void MarkCompleted()
    {
        SetStatus(JobStatus.Completed);
    }

    public void MarkFailed()
    {
        SetStatus(JobStatus.Failed);
    }

    public void MarkCancelled()
    {
        SetStatus(JobStatus.Cancelled);
    }
}