using System;
using DiscordBot.Domain.Seedwork;

namespace DiscordBot.Domain.Entities.Alliances
{
    public class Diplomacy : Entity
    {
        public long _ownerId;
        public virtual Alliance Owner { get; private set; }

        public long _relatedId;
        public virtual Alliance Related { get; private set; }


    }
}
