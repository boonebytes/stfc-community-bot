using System;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;

namespace DiscordBot.Modules
{
    [Group("tdl")]
    public class TdlModule : ModuleBase<SocketCommandContext>
    {
        private readonly Managers.DefendTimes _defendTimes;
        private readonly Responses.Schedule _schedule;

        public TdlModule(Managers.DefendTimes defendTimes, Responses.Schedule schedule)
        {
            _defendTimes = defendTimes;
            _schedule = schedule;
        }

        [Command("today")]
        [Summary("Prints the defense times for today")]
        public async Task TodayAsync()
        {
            var embedMsg = _schedule.GetForDate(DateTime.UtcNow);
            await this.ReplyAsync(embed: embedMsg.Build());
        }

        [Command("tomorrow")]
        [Summary("Prints the defense times for tomorrow")]
        public async Task TomorrowAsync()
        {
            var embedMsg = _schedule.GetForDate(DateTime.UtcNow.AddDays(1));
            await this.ReplyAsync(embed: embedMsg.Build());
        }

        [Command("all")]
        [Summary("Prints the full defense schedule")]
        [Alias("full")]
        public async Task AllAsync()
        {
            var embedMsg = _schedule.GetAll();
            await this.ReplyAsync(embed: embedMsg.Build());
        }
    }
}
