using System.Collections.Generic;
using System.Linq;
using DiscordBot.Domain.Entities.Alliances;
using DiscordBot.Domain.Entities.Request;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DiscordBot.Infrastructure.Repositories
{
    public abstract class BaseRepository
    {
        protected readonly ILogger<BaseRepository> Logger;
        protected readonly BotContext Context;
        protected readonly RequestContext RequestContext;

        protected BaseRepository(ILogger<BaseRepository> logger, BotContext context, RequestContext requestContext)
        {
            Logger = logger;
            Context = context;
            RequestContext = requestContext;
        }

        protected List<long> GetInterestedAlliances(long? allianceId = null)
        {
            List<long> interestedAlliances;
            if (allianceId.HasValue)
            {
                var thisAlliance = Context.Alliances
                    .Include(a => a.Group)
                        .ThenInclude(ag => ag.Alliances)
                    .Include(a => a.AssignedDiplomacy)
                        .ThenInclude(ad => ad.Related)
                    .FirstOrDefault(a => a.Id == allianceId);


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

                var friendlies = Context.Diplomacies
                                .AsQueryable()
                                .Where(d =>
                                        d.Owner == thisAlliance
                                        && (
                                            // d.Relationship == DiplomaticRelation.Friendly
                                            d.Relationship == DiplomaticRelation.Allied
                                        )
                                    );

                interestedAlliances = friendlies
                                            .Select(ag => ag.Related.Id).ToList()
                                            .Union(thisAllianceGroupMembers).ToList();
                if (!interestedAlliances.Contains(thisAlliance.Id))
                    interestedAlliances.Add(thisAlliance.Id);
            }
            else
            {
                interestedAlliances = Context.Alliances
                                            .AsQueryable()
                                            .Select(a => a.Id)
                                            .ToList();
            }
            return interestedAlliances;
        }
    }
}
