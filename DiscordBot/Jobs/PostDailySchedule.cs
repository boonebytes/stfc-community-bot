/*
Copyright 2022 Boonebytes

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/

using Discord;
using Discord.WebSocket;
using DiscordBot.Domain.Entities.Alliances;
using DiscordBot.Domain.Entities.Request;
using DiscordBot.Domain.Shared;
using Quartz;

namespace DiscordBot.Jobs;

[DisallowConcurrentExecution]
public class PostDailySchedule : BaseJob
{
    private readonly DiscordSocketClient _client;
    
    private IAllianceRepository _allianceRepository;
    private Responses.Schedule _scheduleResponse;
    private RequestContext _requestContext;

    public PostDailySchedule(
        ILogger<PostDailySchedule> logger,
        DiscordSocketClient client,
        IAllianceRepository allianceRepository,
        Responses.Schedule scheduleResponse,
        RequestContext requestContext
        ) : base(logger)
    {
        _client = client;
        _allianceRepository = allianceRepository;
        _scheduleResponse = scheduleResponse;
        _requestContext = requestContext;
    }
    
    protected override async Task DoWork(IJobExecutionContext context)
    {
        var data = context.JobDetail.JobDataMap;
        var allianceId = data.GetLong("allianceId");

        if (allianceId == 0) throw new NullReferenceException("Alliance has not been set.");

        _requestContext.Init(allianceId);
        
        var alliance = await _allianceRepository.GetAsync(allianceId);
        Logger.LogInformation("Preparing to post schedule for {Alliance}", alliance.Acronym);

        if (!alliance.GuildId.HasValue || !alliance.DefendSchedulePostChannel.HasValue)
        {
            Logger.LogWarning("Unable to post for {Alliance} - Guild or channel cannot be blank", alliance.Acronym);
            return;
        }

        try
        {
            var guild = _client.GetGuild(alliance.GuildId.Value);
            if (guild == null)
            {
                Logger.LogError(
                    "Unable to post schedule to guild {GuildId} for {Alliance} - Guild not found",
                    alliance.GuildId.Value, alliance.Acronym
                    );
                _allianceRepository.FlagSchedulePosted(alliance);
                await _allianceRepository.UnitOfWork.SaveEntitiesAsync();
            }
            else
            {
                var channel = guild.GetTextChannel(alliance.DefendSchedulePostChannel.Value);
                if (channel == null)
                {
                    Logger.LogError(
                        "Unable to post schedule to guild {GuildId} channel {DefendSchedulePostChannel} for {Alliance} - Guild or channel not found",
                        alliance.GuildId.Value, alliance.DefendSchedulePostChannel.Value, alliance.Acronym);
                    _allianceRepository.FlagSchedulePosted(alliance);
                    await _allianceRepository.UnitOfWork.SaveEntitiesAsync();
                }
                else
                {
                    var channelMessages = await channel.GetMessagesAsync().FlattenAsync();

                    await _scheduleResponse.TryCleanMessages(channel, channelMessages, alliance);
                    await _scheduleResponse.TryUpdateWeeklyMessages(channelMessages, alliance);
                    await TryPinToday(channelMessages, alliance);

                    var embedMsg = _scheduleResponse.GetForDate(DateTime.UtcNow, alliance.Id);
                    await channel.SendMessageAsync(embed: embedMsg.Build());

                    _allianceRepository.FlagSchedulePosted(alliance);
                    await _allianceRepository.UnitOfWork.SaveEntitiesAsync();
                }
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex,
                "An unexpected error occurred while trying to post the schedule to guild {Alliance} ({GuildId}), channel {DefendSchedulePostChannel}",
                alliance.Acronym, alliance.GuildId.Value, alliance.DefendSchedulePostChannel.Value);
            _allianceRepository.FlagSchedulePosted(alliance);
            await _allianceRepository.UnitOfWork.SaveEntitiesAsync();
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
            var message = $"Unable to pin today's schedule for {alliance.Acronym}";
            if (alliance.GuildId.HasValue)
                message += $" in guild {alliance.GuildId.Value}";
            if (alliance.DefendSchedulePostChannel.HasValue)
                message += $" channel {alliance.DefendSchedulePostChannel.Value}";
            Logger.LogWarning(ex, message);
        }
    }
}