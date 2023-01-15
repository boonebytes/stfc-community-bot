﻿/*
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

public partial class ReactMessage  : IAggregateRoot
{
    public ReactMessage(
        ulong fromUserId,
        string fromUsername,
        ulong guildId,
        Alliance alliance,
        string message)
    {
        _reactions = new();
        
        FromUserId = fromUserId;
        FromUsername = fromUsername;
        GuildId = guildId;
        Alliance = alliance;
        _allianceId = alliance.Id;
        Message = message;
    }
}