using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;

namespace DiscordBot
{
    public class Scheduler
    {
        private readonly ILogger<Scheduler> _logger;
        //private readonly Managers.DefendTimes _defendTimes;
        private readonly DiscordSocketClient _client;
        private readonly Managers.DiscordServers _discordServers;
        private readonly Responses.Schedule _schedule;

        public Scheduler(
                ILogger<Scheduler> logger,
                //Managers.DefendTimes defendTimes,
                DiscordSocketClient client,
                Managers.DiscordServers discordServers,
                Responses.Schedule schedule
            )
        {
            _logger = logger;
            //_defendTimes = defendTimes;
            _client = client;
            _discordServers = discordServers;
            _schedule = schedule;
        }

        public async Task Run(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var nextPost = _discordServers.Servers
                    .OrderBy(s => s.NextScheduledPost)
                    .First();


                if (nextPost.NextScheduledPost > DateTime.UtcNow)
                {
                    var delay = nextPost.NextScheduledPost - DateTime.UtcNow;
                    await Task.Delay(delay, stoppingToken);
                }
                if (!stoppingToken.IsCancellationRequested && DateTime.UtcNow >= nextPost.NextScheduledPost)
                {
                    var guild = _client.GetGuild(nextPost.GuildId);
                    if (guild == null)
                    {
                        _logger.LogError($"Unable to log to guild {nextPost.GuildId} channel {nextPost.PostToChannel} for {nextPost.AllianceAcronym} - Guild not found");
                        nextPost.FlagPosted();
                    }
                    else
                    {
                        var channel = guild.GetTextChannel(nextPost.PostToChannel);
                        if (channel == null)
                        {
                            _logger.LogError($"Unable to log to guild {nextPost.GuildId} channel {nextPost.PostToChannel} for {nextPost.AllianceAcronym} - Guild or channel not found");
                            nextPost.FlagPosted();
                        }
                        else
                        {
                            var embedMsg = _schedule.GetForDate(DateTime.UtcNow);
                            await channel.SendMessageAsync(embed: embedMsg.Build());
                            nextPost.FlagPosted();
                        }
                    }
                }
            }
        }
    }
}
