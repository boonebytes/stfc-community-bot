using Discord;
using Discord.WebSocket;
using DiscordBot.Domain.Entities.Alliances;
using DiscordBot.Domain.Entities.Zones;
using DiscordBot.Domain.Shared;

namespace DiscordBot.Models.Scheduler;

public class PostDailySchedule : BaseJob
{
    public long AllianceId { get; init; } = 0;

    public override string Id
    {
        get { return nameof(PostDailySchedule) + "-" + AllianceId; }
    }

    private readonly ILogger<PostDailySchedule> _logger;
    private readonly DiscordSocketClient _client;

    private IAllianceRepository _allianceRepository;
    private IZoneRepository _zoneRepository;
    private Responses.Schedule _scheduleResponse;

    public PostDailySchedule(IServiceProvider serviceProvider, long allianceId) : base(serviceProvider)
    {
        AllianceId = allianceId;
        _logger = _serviceProvider.GetService<ILogger<PostDailySchedule>>();
        _client = _serviceProvider.GetService<DiscordSocketClient>();
    }
    
    public override async Task SetNextExecutionTime(IServiceScope serviceScope)
    {
        var alliance = await _allianceRepository.GetAsync(AllianceId);
        if (alliance.NextScheduledPost.HasValue && alliance.NextScheduledPost.Value > DateTime.Now)
        {
            NextExecutionTime = alliance.NextScheduledPost.Value;
        }
        else
        {
            NextExecutionTime = null;
        }
    }

    public override async Task DoWork(CancellationToken cancellationToken)
    {
        if (AllianceId == 0) throw new NullReferenceException("Alliance has not been set.");
            
        using var thisServiceScope = _serviceProvider.CreateScope();
        _allianceRepository = thisServiceScope.ServiceProvider.GetService<IAllianceRepository>();
        _zoneRepository = thisServiceScope.ServiceProvider.GetService<IZoneRepository>();
        _scheduleResponse = thisServiceScope.ServiceProvider.GetService<Responses.Schedule>();

        var alliance = await _allianceRepository.GetAsync(AllianceId);
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
                await _allianceRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
            }
            else
            {
                var channel = guild.GetTextChannel(alliance.DefendSchedulePostChannel.Value);
                if (channel == null)
                {
                    _logger.LogError(
                        $"Unable to post schedule to guild {alliance.GuildId.Value} channel {alliance.DefendSchedulePostChannel.Value} for {alliance.Acronym} - Guild or channel not found");
                    _allianceRepository.FlagSchedulePosted(alliance);
                    await _allianceRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
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
                    await _allianceRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                $"An unexpected error occurred while trying to post the schedule to guild {alliance.Acronym} ({alliance.GuildId.Value}), channel {alliance.DefendSchedulePostChannel.Value}");
            _allianceRepository.FlagSchedulePosted(alliance);
            await _allianceRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
        }
        
        await SetNextExecutionTime(thisServiceScope);
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