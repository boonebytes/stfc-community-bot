﻿using System;
using DiscordBot.Domain.Entities.Alliances;
using DiscordBot.Domain.Seedwork;

namespace DiscordBot.Domain.Entities.Zones
{
    public partial class Zone : IAggregateRoot
    {
        public Zone(
                long id,
                string name,
                int level,
                Alliance owner,
                string threats,
                string defendUtcDayOfWeek,
                string defendUtcTime,
                string notes
            )
        {
            Id = id;
            this.Update(
                name,
                level,
                owner,
                threats,
                defendUtcDayOfWeek,
                defendUtcTime,
                notes
                );
        }

        public void Update(
                string name,
                int level,
                Alliance owner,
                string threats,
                string defendUtcDayOfWeek,
                string defendUtcTime,
                string notes
            )
        {
            Name = name;
            Level = level;
            Owner = owner;
            Threats = threats;
            DefendUtcDayOfWeek = defendUtcDayOfWeek;
            DefendUtcTime = defendUtcTime;
            Notes = notes;
        }
    }
}