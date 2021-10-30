using System;
using System.Collections.Generic;
using System.Linq;
using DiscordBot.Domain.Exceptions;
using DiscordBot.Domain.Seedwork;

namespace DiscordBot.Domain.Entities.Zones
{
    public class Resource : Enumeration
    {
        public static Resource Unspecified = new(0, nameof(Unspecified), "Unspecified");

        public static Resource Parasteel = new(1, nameof(Parasteel), "Parasteel");
        public static Resource Tritanium = new(2, nameof(Tritanium), "Tritanium");
        public static Resource Dilithium = new(3, nameof(Dilithium), "Dilithium");

        public static Resource IsogenTier1 = new(11, nameof(IsogenTier1), "Isogen Tier 1");
        public static Resource IsogenTier2 = new(12, nameof(IsogenTier2), "Isogen Tier 2");
        public static Resource IsogenTier3 = new(13, nameof(IsogenTier3), "Isogen Tier 3");

        public static Resource GasTier3 = new(23, nameof(GasTier3), "Gas Tier 3");
        public static Resource GasTier4 = new(24, nameof(GasTier4), "Gas Tier 4");
        public static Resource CrystalTier3 = new(33, nameof(CrystalTier3), "Crystal Tier 3");
        public static Resource CrystalTier4 = new(34, nameof(CrystalTier4), "Crystal Tier 4");
        public static Resource OreTier3 = new(43, nameof(OreTier3), "Ore Tier 3");
        public static Resource OreTier4 = new(44, nameof(OreTier4), "Ore Tier 4");

        public virtual string Label { get; private set; }

        public Resource(int id, string name, string label)
            : base(id, name)
        {
            this.Label = label;
        }

        public static IEnumerable<Resource> List() =>
            new[] { Unspecified, Parasteel, Tritanium, Dilithium,
                    IsogenTier1, IsogenTier2, IsogenTier3,
                    GasTier3, GasTier4,
                    CrystalTier3, CrystalTier4,
                    OreTier3, OreTier4 };

        public static Resource FromName(string name)
        {
            var res = List()
                .SingleOrDefault(s => String.Equals(s.Name, name, StringComparison.CurrentCultureIgnoreCase));

            if (res == null)
            {
                throw new BotDomainException($"Possible values for Resource: {String.Join(",", List().Select(s => s.Name))}");
            }

            return res;
        }

        public static Resource From(int id)
        {
            var res = List().SingleOrDefault(s => s.Id == id);

            if (res == null)
            {
                throw new BotDomainException($"Possible values for Resource: {String.Join(",", List().Select(s => s.Name))}");
            }

            return res;
        }
    }
}
