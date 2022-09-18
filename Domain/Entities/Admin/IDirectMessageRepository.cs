using System.Threading.Tasks;
using DiscordBot.Domain.Seedwork;

namespace DiscordBot.Domain.Entities.Admin
{
    public interface IDirectMessageRepository : IRepository<DirectMessage>
    {
        DirectMessage Add(DirectMessage directMessage);

        Task<DirectMessage> GetAsync(long id);
    }
}
