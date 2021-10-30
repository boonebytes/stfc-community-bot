using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using DiscordBot.Domain.Entities.Alliances;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace DiscordBot
{
    public class Scheduler
    {
        private readonly ILogger<Scheduler> _logger;
        //private readonly Managers.DefendTimes _defendTimes;
        private readonly DiscordSocketClient _client;
        private readonly IServiceProvider _serviceProvider;

        public Scheduler(
                ILogger<Scheduler> logger,
                //Managers.DefendTimes defendTimes,
                DiscordSocketClient client,
                IServiceProvider serviceProvider
            )
        {
            _logger = logger;
            //_defendTimes = defendTimes;
            _client = client;
            _serviceProvider = serviceProvider;
        }

        public async Task Run(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using (var thisServiceScope = _serviceProvider.CreateScope())
                {
                    IAllianceRepository allianceRepository = thisServiceScope.ServiceProvider.GetService<IAllianceRepository>();
                    Responses.Schedule scheduleResponse = thisServiceScope.ServiceProvider.GetService<Responses.Schedule>();
                    var nextPost = allianceRepository.GetNextOnPostSchedule();


                    if (nextPost.NextScheduledPost > DateTime.UtcNow)
                    {
                        var delay = nextPost.NextScheduledPost - DateTime.UtcNow;
                        if (delay.TotalMinutes > 30) delay = new TimeSpan(0, 30, 0);
                        await Task.Delay(delay, stoppingToken);
                    }
                    if (!stoppingToken.IsCancellationRequested && DateTime.UtcNow >= nextPost.NextScheduledPost)
                    {
                        try
                        {

                            var guild = _client.GetGuild(nextPost.GuildId.Value);
                            if (guild == null)
                            {
                                _logger.LogError($"Unable to post schedule to guild {nextPost.GuildId.Value} channel {nextPost.DefendSchedulePostChannel.Value} for {nextPost.Acronym} - Guild not found");
                                allianceRepository.FlagSchedulePosted(nextPost);
                                await allianceRepository.UnitOfWork.SaveEntitiesAsync();
                            }
                            else
                            {
                                var channel = guild.GetTextChannel(nextPost.DefendSchedulePostChannel.Value);
                                if (channel == null)
                                {
                                    _logger.LogError($"Unable to post schedule to guild {nextPost.GuildId.Value} channel {nextPost.DefendSchedulePostChannel.Value} for {nextPost.Acronym} - Guild or channel not found");
                                    allianceRepository.FlagSchedulePosted(nextPost);
                                    await allianceRepository.UnitOfWork.SaveEntitiesAsync();
                                }
                                else
                                {
                                    var myUserId = _client.CurrentUser.Id;

                                    try
                                    {
                                        var channelMessages = await channel.GetMessagesAsync().FlattenAsync();
                                        var myMessages = channelMessages.Where(m =>
                                                !m.IsPinned
                                                && m.Author.Id == _client.CurrentUser.Id
                                                && (DateTimeOffset.UtcNow - m.Timestamp).TotalDays <= 14
                                            )
                                            .ToList();
                                        await channel.DeleteMessagesAsync(myMessages);
                                    }
                                    catch (Exception ex)
                                    {
                                        _logger.LogWarning(ex, $"Unable to delete messages for new schedule for {nextPost.Acronym} in guild {nextPost.GuildId.Value} channel {nextPost.DefendSchedulePostChannel.Value}");
                                    }


                                    var embedMsg = scheduleResponse.GetForDate(DateTime.UtcNow);
                                    await channel.SendMessageAsync(embed: embedMsg.Build());

                                    allianceRepository.FlagSchedulePosted(nextPost);
                                    await allianceRepository.UnitOfWork.SaveEntitiesAsync();
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, $"An unexpected error occurred while trying to post the schedule to guild {nextPost.Acronym} ({nextPost.GuildId}), channel {nextPost.DefendSchedulePostChannel}");
                            allianceRepository.FlagSchedulePosted(nextPost);
                            await allianceRepository.UnitOfWork.SaveEntitiesAsync();
                        }
                    }
                }
            }
        }
    }
}
