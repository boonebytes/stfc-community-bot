using Discord;
using Discord.WebSocket;
using DiscordBot.Domain.Entities.Alliances;
using DiscordBot.Domain.Entities.Request;
using DiscordBot.Domain.Entities.Zones;
using DiscordBot.Domain.Shared;
using Quartz;

namespace DiscordBot.Jobs;

[DisallowConcurrentExecution]
public class PostDailySchedule : BaseJob
{
    private readonly DiscordSocketClient _client;
    
    private IAllianceRepository _allianceRepository;
    private IZoneRepository _zoneRepository;
    private Responses.Schedule _scheduleResponse;
    private RequestContext _requestContext;

    public PostDailySchedule(
        ILogger<PostDailySchedule> logger,
        DiscordSocketClient client,
        IAllianceRepository allianceRepository,
        IZoneRepository zoneRepository,
        Responses.Schedule scheduleResponse,
        RequestContext requestContext
        ) : base(logger)
    {
        _client = client;
        _allianceRepository = allianceRepository;
        _zoneRepository = zoneRepository;
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
        _logger.LogInformation($"Preparing to post schedule for {alliance.Acronym}");

        if (!alliance.GuildId.HasValue || !alliance.DefendSchedulePostChannel.HasValue)
        {
            _logger.LogWarning($"Unable to post for {alliance.Acronym} - Guild or channel cannot be blank");
            return;
        }

        try
        {
            var guild = _client.GetGuild(alliance.GuildId.Value);
            if (guild == null)
            {
                _logger.LogError(
                    $"Unable to post schedule to guild {alliance.GuildId.Value} for {alliance.Acronym} - Guild not found");
                _allianceRepository.FlagSchedulePosted(alliance);
                await _allianceRepository.UnitOfWork.SaveEntitiesAsync();
            }
            else
            {
                var channel = guild.GetTextChannel(alliance.DefendSchedulePostChannel.Value);
                if (channel == null)
                {
                    _logger.LogError(
                        $"Unable to post schedule to guild {alliance.GuildId.Value} channel {alliance.DefendSchedulePostChannel.Value} for {alliance.Acronym} - Guild or channel not found");
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
            _logger.LogError(ex,
                $"An unexpected error occurred while trying to post the schedule to guild {alliance.Acronym} ({alliance.GuildId.Value}), channel {alliance.DefendSchedulePostChannel.Value}");
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
            _logger.LogWarning(ex, message);
        }
    }
}