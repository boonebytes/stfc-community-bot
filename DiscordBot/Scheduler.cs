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

        private IAllianceRepository _allianceRepository;
        private IZoneRepository _zoneRepository;
        private Responses.Schedule _scheduleResponse;

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

        public async Task Monitor(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var now = DateTime.UtcNow;
                var delay = new DateTime(now.Year, now.Month, now.Day, 6, 0, 0, DateTimeKind.Utc).AddDays(1) - DateTime.UtcNow;
                await Task.Delay(delay, stoppingToken);

                if (!stoppingToken.IsCancellationRequested)
                {
                    using (var thisServiceScope = _serviceProvider.CreateScope())
                    {
                        _allianceRepository = thisServiceScope.ServiceProvider.GetService<IAllianceRepository>();
                        _zoneRepository = thisServiceScope.ServiceProvider.GetService<IZoneRepository>();

                        await _allianceRepository.InitPostSchedule();
                        await _zoneRepository.InitZones();
                    }
                }
            }
        }

        public async Task Run(CancellationToken stoppingToken, int pollingIntervalSeconds)
        {
            _ = Monitor(stoppingToken);
            while (!stoppingToken.IsCancellationRequested)
            {
                using (var thisServiceScope = _serviceProvider.CreateScope())
                {
                    _allianceRepository = thisServiceScope.ServiceProvider.GetService<IAllianceRepository>();
                    _zoneRepository = thisServiceScope.ServiceProvider.GetService<IZoneRepository>();
                    _scheduleResponse = thisServiceScope.ServiceProvider.GetService<Responses.Schedule>();
                    var nextAllianceToPost = _allianceRepository.GetNextOnPostSchedule();

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
                                _allianceRepository.FlagSchedulePosted(nextAllianceToPost);
                                await _allianceRepository.UnitOfWork.SaveEntitiesAsync();
                            }
                            else
                            {
                                var channel = guild.GetTextChannel(nextAllianceToPost.DefendSchedulePostChannel.Value);
                                if (channel == null)
                                {
                                    _logger.LogError($"Unable to post schedule to guild {nextAllianceToPost.GuildId.Value} channel {nextAllianceToPost.DefendSchedulePostChannel.Value} for {nextAllianceToPost.Acronym} - Guild or channel not found");
                                    _allianceRepository.FlagSchedulePosted(nextAllianceToPost);
                                    await _allianceRepository.UnitOfWork.SaveEntitiesAsync();
                                }
                                else
                                {
                                    var channelMessages = await channel.GetMessagesAsync().FlattenAsync();

                                    await _scheduleResponse.TryCleanMessages(channel, channelMessages, nextAllianceToPost);
                                    await _scheduleResponse.TryUpdateWeeklyMessages(channelMessages, nextAllianceToPost);
                                    await TryPinToday(channelMessages, nextAllianceToPost);
                                    
                                    var embedMsg = _scheduleResponse.GetForDate(DateTime.UtcNow, nextAllianceToPost.Id);
                                    await channel.SendMessageAsync(embed: embedMsg.Build());

                                    _allianceRepository.FlagSchedulePosted(nextAllianceToPost);
                                    await _allianceRepository.UnitOfWork.SaveEntitiesAsync();
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, $"An unexpected error occurred while trying to post the schedule to guild {nextAllianceToPost.Acronym} ({nextAllianceToPost.GuildId}), channel {nextAllianceToPost.DefendSchedulePostChannel}");
                            _allianceRepository.FlagSchedulePosted(nextAllianceToPost);
                            await _allianceRepository.UnitOfWork.SaveEntitiesAsync();
                        }
                    }
                }
            }
        }

        protected async Task TryPinToday(IEnumerable<IMessage> channelMessages, Alliance alliance)
        {
            try
            {
                var todayShortPosts = channelMessages.Where(m =>
                        m.Author.Id == _client.CurrentUser.Id
                        && m.Embeds.Count == 0
                        && m.Content.StartsWith("**__" + DateTime.Now.ToEasternTime().DayOfWeek.ToString() + "__**")
                    )
                    .ToList();
                if (todayShortPosts.Count == 1)
                {
                    var todayShortPost = (Discord.Rest.RestUserMessage)todayShortPosts.First();
                    await todayShortPost.PinAsync();
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, $"Unable to pin today's schedule for {alliance.Acronym} in guild {alliance.GuildId.Value} channel {alliance.DefendSchedulePostChannel.Value}");
            }
        }
    }
}
