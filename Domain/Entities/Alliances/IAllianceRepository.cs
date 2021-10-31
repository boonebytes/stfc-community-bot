using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DiscordBot.Domain.Seedwork;

namespace DiscordBot.Domain.Entities.Alliances
{
    public interface IAllianceRepository : IRepository<Alliance>
    {
        Alliance Add(Alliance alliance);

        Alliance Update(Alliance alliance);

        Task<Alliance> GetAsync(long id);

        Task<List<Alliance>> GetAllAsync();

        List<Alliance> GetAllWithServers();

        Alliance GetNextOnPostSchedule();

        Alliance FindFromGuildId(ulong id);

        Alliance FlagSchedulePosted(Alliance alliance);

        Task InitPostSchedule();
    }
}
