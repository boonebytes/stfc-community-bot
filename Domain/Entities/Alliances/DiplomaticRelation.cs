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
using System.Linq;
using DiscordBot.Domain.Exceptions;
using DiscordBot.Domain.Seedwork;

namespace DiscordBot.Domain.Entities.Alliances
{
    public class DiplomaticRelation : Enumeration
    {
        public static DiplomaticRelation Unspecified = new(0, nameof(Unspecified));
        public static DiplomaticRelation Enemy = new(-99, nameof(Enemy));
        public static DiplomaticRelation Untrusted = new(-1, nameof(Untrusted));
        public static DiplomaticRelation Neutral = new(1, nameof(Neutral));
        public static DiplomaticRelation Friendly = new(2, nameof(Friendly));
        public static DiplomaticRelation Allied = new(3, nameof(Allied));

        public DiplomaticRelation(int id, string name)
            : base(id, name)
        {
        }

        public static IEnumerable<DiplomaticRelation> List() =>
            new[] { Unspecified, Enemy, Untrusted, Neutral, Friendly, Allied };

        public static DiplomaticRelation FromName(string name)
        {
            var res = List()
                .SingleOrDefault(s => String.Equals(s.Name, name, StringComparison.CurrentCultureIgnoreCase));

            if (res == null)
            {
                throw new BotDomainException($"Possible values for DiplomaticRelation: {String.Join(",", List().Select(s => s.Name))}");
            }

            return res;
        }

        public static DiplomaticRelation From(int id)
        {
            var res = List().SingleOrDefault(s => s.Id == id);

            if (res == null)
            {
                throw new BotDomainException($"Possible values for DiplomaticRelation: {String.Join(",", List().Select(s => s.Name))}");
            }

            return res;
        }
    }
}
