using System;
using DiscordBot.Domain.Seedwork;

namespace DiscordBot.Domain.Entities.Admin
{
    public partial class DirectMessage : Entity
    {
        public DirectMessage()
        {
            ReceivedTimestamp = DateTime.UtcNow;
        }

        public virtual DateTime ReceivedTimestamp { get; private set; }
        public virtual ulong FromUser { get; private set; }
        public virtual string CommonServers { get; private set; }
        public virtual string Message { get; private set; }
    }
}
