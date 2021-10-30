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
        public static DiplomaticRelation Enemy = new(-1, nameof(Enemy));
        public static DiplomaticRelation Neutral = new(1, nameof(Neutral));
        public static DiplomaticRelation Friendly = new(2, nameof(Friendly));
        public static DiplomaticRelation Allied = new(3, nameof(Allied));

        public DiplomaticRelation(int id, string name)
            : base(id, name)
        {
        }

        public static IEnumerable<DiplomaticRelation> List() =>
            new[] { Unspecified, Enemy, Neutral, Friendly, Allied };

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
