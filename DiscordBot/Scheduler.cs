using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using DiscordBot.Domain.Entities.Alliances;
using DiscordBot.Domain.Entities.Zones;
using DiscordBot.Domain.Shared;
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

        private IAllianceRepository allianceRepository;
        private IZoneRepository zoneRepository;
        private Responses.Schedule scheduleResponse;

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

        public async Task Run(CancellationToken stoppingToken, int pollingIntervalSeconds)
        {
            using (var thisServiceScope = _serviceProvider.CreateScope())
            {
                allianceRepository = thisServiceScope.ServiceProvider.GetService<IAllianceRepository>();
                zoneRepository = thisServiceScope.ServiceProvider.GetService<IZoneRepository>();
                scheduleResponse = thisServiceScope.ServiceProvider.GetService<Responses.Schedule>();

                while (!stoppingToken.IsCancellationRequested)
                {
                    var nextAllianceToPost = allianceRepository.GetNextOnPostSchedule();

                    if (nextAllianceToPost.NextScheduledPost.Value > DateTime.UtcNow)
                    {
                        var delay = nextAllianceToPost.NextScheduledPost.Value - DateTime.UtcNow;
                        if (delay.TotalSeconds > pollingIntervalSeconds) delay = new TimeSpan(0, 0, pollingIntervalSeconds);
                        await Task.Delay(delay, stoppingToken);
                    }
                    

                    if (!stoppingToken.IsCancellationRequested && DateTime.UtcNow >= nextAllianceToPost.NextScheduledPost)
                    {
                        _logger.LogInformation($"Preparing to post schedule for {nextAllianceToPost.Acronym}");
                        try
                        {
                            var guild = _client.GetGuild(nextAllianceToPost.GuildId.Value);
                            if (guild == null)
                            {
                                _logger.LogError($"Unable to post schedule to guild {nextAllianceToPost.GuildId.Value} channel {nextAllianceToPost.DefendSchedulePostChannel.Value} for {nextAllianceToPost.Acronym} - Guild not found");
                                allianceRepository.FlagSchedulePosted(nextAllianceToPost);
                                await allianceRepository.UnitOfWork.SaveEntitiesAsync();
                            }
                            else
                            {
                                var channel = guild.GetTextChannel(nextAllianceToPost.DefendSchedulePostChannel.Value);
                                if (channel == null)
                                {
                                    _logger.LogError($"Unable to post schedule to guild {nextAllianceToPost.GuildId.Value} channel {nextAllianceToPost.DefendSchedulePostChannel.Value} for {nextAllianceToPost.Acronym} - Guild or channel not found");
                                    allianceRepository.FlagSchedulePosted(nextAllianceToPost);
                                    await allianceRepository.UnitOfWork.SaveEntitiesAsync();
                                }
                                else
                                {
                                    var channelMessages = await channel.GetMessagesAsync().FlattenAsync();

                                    await TryCleanMessages(channel, channelMessages, nextAllianceToPost);
                                    await TryUpdateYesterdayMessage(channelMessages, nextAllianceToPost);
                                    
                                    var embedMsg = scheduleResponse.GetForDate(DateTime.UtcNow, nextAllianceToPost.Id);
                                    await channel.SendMessageAsync(embed: embedMsg.Build());

                                    allianceRepository.FlagSchedulePosted(nextAllianceToPost);
                                    await allianceRepository.UnitOfWork.SaveEntitiesAsync();
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, $"An unexpected error occurred while trying to post the schedule to guild {nextAllianceToPost.Acronym} ({nextAllianceToPost.GuildId}), channel {nextAllianceToPost.DefendSchedulePostChannel}");
                            allianceRepository.FlagSchedulePosted(nextAllianceToPost);
                            await allianceRepository.UnitOfWork.SaveEntitiesAsync();
                        }
                    }
                }
            }
        }

        protected async Task TryCleanMessages(SocketTextChannel channel, IEnumerable<IMessage> channelMessages, Alliance alliance)
        {
            try
            {
                var myMessages = channelMessages.Where(m =>
                        !m.IsPinned
                        && m.Author.Id == _client.CurrentUser.Id
                        && m.Embeds.Count == 1
                        && m.Embeds.First().Title.StartsWith("Defend Schedule for ")
                        && (DateTimeOffset.UtcNow - m.Timestamp).TotalDays <= 14
                    )
                    .ToList();
                await channel.DeleteMessagesAsync(myMessages);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, $"Unable to delete messages for new schedule for {alliance.Acronym} in guild {alliance.GuildId.Value} channel {alliance.DefendSchedulePostChannel.Value}");
            }
        }

        protected async Task TryUpdateYesterdayMessage(IEnumerable<IMessage> channelMessages, Alliance alliance)
        {
            try
            {
                var yesterdayShortPosts = channelMessages.Where(m =>
                        m.Author.Id == _client.CurrentUser.Id
                        && m.Embeds.Count == 0
                        && m.Content.StartsWith("**__" + DateTime.Now.ToEasternTime().AddDays(-1).DayOfWeek.ToString() + "__**")
                    )
                    .ToList();

                if (yesterdayShortPosts.Count == 1)
                {
                    var yesterdaysShortPost = (Discord.Rest.RestUserMessage)yesterdayShortPosts.First();
                    var yesterdayDefends = zoneRepository.GetNext24Hours(
                                                    DateTime.Now.AddDays(6),
                                                    alliance.Id)
                                                .OrderBy(z => z.NextDefend)
                                                .ToList();
                    await yesterdaysShortPost.ModifyAsync(msg =>
                            msg.Content = scheduleResponse.GetDayScheduleAsString(yesterdayDefends, DateTime.Now.ToEasternTime().AddDays(-1).DayOfWeek, true)
                        );

                }
                else
                {
                    _logger.LogError($"Unable to find mesage to edit yesterday's schedule for {alliance.Acronym} in guild {alliance.GuildId.Value} channel {alliance.DefendSchedulePostChannel.Value}. Records returned: {yesterdayShortPosts.Count}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, $"Unable to edit message for yesterday's schedule for {alliance.Acronym} in guild {alliance.GuildId.Value} channel {alliance.DefendSchedulePostChannel.Value}");
            }
        }
    }
}
