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
using System.Diagnostics;
using DiscordBot.Domain.Entities.Alliances;
using DiscordBot.Domain.Seedwork;

namespace DiscordBot.Domain.Entities.Admin;

public partial class ReactMessage  : Entity
{
    protected ReactMessage()
    {
    }

    public virtual DateTime? Posted { get; private set; }
    public virtual ulong FromUserId { get; private set; }
    public virtual string FromUsername { get; private set; }
    public virtual ulong GuildId { get; private set; }
    public virtual ulong ChannelId { get; private set; }
    public virtual ulong? MessageId { get; private set; }
    public virtual string Message { get; private set; }
    public virtual string ResponseText { get; private set; }
    
    private long _allianceId;
    public virtual Alliance Alliance { get; private set; }

    private readonly List<Reaction> _reactions;
    public IReadOnlyCollection<Reaction> Reactions => _reactions.AsReadOnly();

    public void MarkPosted(ulong messageId)
    {
        MessageId = messageId;
        Posted = DateTime.UtcNow;
    }

    public void AddReaction(ulong userId, string username, string nickname)
    {
        var reaction = new Reaction(userId, username, nickname, this);
        _reactions.Add(reaction);
    }
}