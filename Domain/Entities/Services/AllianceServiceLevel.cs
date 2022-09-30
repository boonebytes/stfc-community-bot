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

namespace DiscordBot.Domain.Entities.Services
{
    public class AllianceServiceLevel : Enumeration
    {
        public static AllianceServiceLevel Undefined = new(0, nameof(Undefined), "Undefined");
        public static AllianceServiceLevel Disabled = new(1, nameof(Disabled), "Disabled");
        public static AllianceServiceLevel Redundant = new(2, nameof(Redundant), "Redundant");
        public static AllianceServiceLevel Desired = new(3, nameof(Desired), "Desired");
        public static AllianceServiceLevel Preferred = new(4, nameof(Preferred), "Preferred");
        public static AllianceServiceLevel Basic = new(5, nameof(Basic), "Basic");

        public virtual string Label { get; private set; }

        public AllianceServiceLevel(int id, string name, string label)
            : base(id, name)
        {
            this.Label = label;
        }

        public static IEnumerable<AllianceServiceLevel> List() =>
            new[] { Undefined, Disabled, Redundant, Desired, Preferred, Basic };

        public static AllianceServiceLevel FromName(string name)
        {
            var sl = List()
                .SingleOrDefault(s => String.Equals(s.Name, name, StringComparison.CurrentCultureIgnoreCase));

            if (sl == null)
            {
                throw new BotDomainException($"Possible values for Service Level: {String.Join(",", List().Select(s => s.Name))}");
            }

            return sl;
        }

        public static AllianceServiceLevel From(int id)
        {
            var sl = List().SingleOrDefault(s => s.Id == id);

            if (sl == null)
            {
                throw new BotDomainException($"Possible values for Service Level: {String.Join(",", List().Select(s => s.Name))}");
            }

            return sl;
        }
    }
}
