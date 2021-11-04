﻿using System;
using System.Collections.Generic;
using System.Linq;
using DiscordBot.Domain.Entities.Alliances;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DiscordBot.Infrastructure.Repositories
{
    public abstract class BaseRepository
    {
        protected readonly ILogger<BaseRepository> _logger;
        protected readonly BotContext _context;

        public BaseRepository(ILogger<BaseRepository> logger, BotContext context)
        {
            _logger = logger;
            _context = context;
        }

        protected List<long> GetInterestedAlliances(long? allianceId = null)
        {
            List<long> interestedAlliances;
            if (allianceId.HasValue)
            {
                var thisAlliance = _context.Alliances
                    .Include(a => a.Group)
                        .ThenInclude(ag => ag.Alliances)
                    .Include(a => a.AssignedDiplomacy)
                        .ThenInclude(ad => ad.Related)
                    .Where(a => a.Id == allianceId)
                    .FirstOrDefault();


                List<long> thisAllianceGroupMembers;

                if (thisAlliance.Group == null)
                {
                    thisAllianceGroupMembers = new();
                }
                else
                {
                    thisAllianceGroupMembers = thisAlliance.Group.Alliances
                                                .Select(gm => gm.Id).ToList();
                }

                var friendlies = _context.Diplomacies
                                .Where(d =>
                                        d.Owner == thisAlliance
                                        && (
                                            d.Relationship == DiplomaticRelation.Friendly
                                            || d.Relationship == DiplomaticRelation.Allied
                                        )
                                    );

                interestedAlliances = friendlies
                                            .Select(ag => ag.Related.Id).ToList()
                                            .Union(thisAllianceGroupMembers).ToList();
            }
            else
            {
                interestedAlliances = _context.Alliances
                                            .Select(a => a.Id)
                                            .ToList();
            }
            return interestedAlliances;
        }
    }
}