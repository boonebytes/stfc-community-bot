using System;
using DiscordBot.Domain.Seedwork;

namespace DiscordBot.Domain.Entities.Alliances
{
    public class Diplomacy : Entity
    {
        protected long _ownerId;
        public virtual Alliance Owner { get; private set; }

        protected long _relatedId;
        public virtual Alliance Related { get; private set; }

        protected int _relationshipId;
        public virtual DiplomaticRelation Relationship { get; private set; }
    }
}
