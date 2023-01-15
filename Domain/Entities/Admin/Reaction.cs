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
using DiscordBot.Domain.Seedwork;

namespace DiscordBot.Domain.Entities.Admin;

public class Reaction : Entity
{
    public virtual ulong UserId { get; private set; }
    public virtual string Username { get; private set; }
    public virtual string Nickname { get; private set; }
    public virtual DateTime ReactionReceived { get; private set; }

    private long _reactMessageId;
    public virtual ReactMessage Message { get; private set; }

    protected Reaction()
    {
    }
    
    internal Reaction(ulong userId, string username, string nickname, ReactMessage message)
    {
        UserId = userId;
        Username = username;
        Nickname = nickname;
        ReactionReceived = DateTime.UtcNow;
        Message = message;
        _reactMessageId = message.Id;
    }
}