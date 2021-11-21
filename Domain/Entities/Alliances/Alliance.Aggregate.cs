﻿using System;
using DiscordBot.Domain.Seedwork;

namespace DiscordBot.Domain.Entities.Alliances
{
    public partial class Alliance : IAggregateRoot
    {
        public Alliance(
                long id,
                string name,
                string acronym,
                AllianceGroup group,
                ulong? guildId,
                ulong? defendSchedulePostChannelId,
                string defendSchedulePostTime
            )
        {
            Id = id;
            this.Update(
                name,
                acronym,
                group,
                guildId,
                defendSchedulePostChannelId,
                defendSchedulePostTime
                );
        }

        public void Update(
                string name,
                string acronym,
                AllianceGroup group,
                ulong? guildId,
                ulong? defendSchedulePostChannelId,
                string defendSchedulePostTime
            )
        {
            Name = name;
            Acronym = acronym;
            Group = group;
            GuildId = guildId;
            DefendSchedulePostChannel = defendSchedulePostChannelId;
            DefendSchedulePostTime = defendSchedulePostTime;
        }

    }
}