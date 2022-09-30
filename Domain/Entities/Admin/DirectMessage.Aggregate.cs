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

namespace DiscordBot.Domain.Entities.Admin
{
    public partial class DirectMessage : IAggregateRoot
    {
        public DirectMessage(
            ulong fromUser,
            string message)
        {
            ReceivedTimestamp = DateTime.UtcNow;
            FromUser = fromUser;
            Message = message;
        }

        public void AddServer(ulong guildId, string name)
        {
            var entry = name + " (" + guildId.ToString() + ")";
            if (string.IsNullOrEmpty(CommonServers))
            {
                CommonServers = entry;
            }
            else
            {
                CommonServers += ", " + entry;
            }
        }
    }
}
