using System;
using System.Collections.Generic;
using DiscordBot.Domain.Seedwork;

namespace DiscordBot.Domain.Entities.Alliances
{
    public class AllianceGroup : Entity
    {
        public AllianceGroup()
        {
        }

        public virtual string Name { get; private set; }

        private readonly List<Alliance> _alliances;
        public IReadOnlyCollection<Alliance> Alliances => _alliances;
    }
}
