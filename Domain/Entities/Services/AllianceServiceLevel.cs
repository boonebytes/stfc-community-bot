using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
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
