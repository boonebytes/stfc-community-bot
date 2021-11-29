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
