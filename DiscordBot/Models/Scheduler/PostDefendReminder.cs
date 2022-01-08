using System.Text;
using Discord;
using Discord.WebSocket;
using DiscordBot.Domain.Entities.Alliances;
using DiscordBot.Domain.Entities.Zones;
using DiscordBot.Domain.Shared;
using MediatR;

namespace DiscordBot.Models.Scheduler;

public class PostDefendReminder : BaseJob
{
    public long ZoneId { get; init; } = 0;

    public override string Id
    {
        get { return nameof(PostDefendReminder) + "-" + ZoneId; }
    }

    private readonly ILogger<PostDefendReminder> _logger;
    private readonly DiscordSocketClient _client;

    private IAllianceRepository _allianceRepository;
    private IZoneRepository _zoneRepository;
    private Responses.Schedule _scheduleResponse;

    public PostDefendReminder(IServiceProvider serviceProvider, long zoneId) : base(serviceProvider)
    {
        ZoneId = zoneId;
        _logger = _serviceProvider.GetService<ILogger<PostDefendReminder>>();
        _client = _serviceProvider.GetService<DiscordSocketClient>();
    }

    public override async Task SetNextExecutionTime(IServiceScope serviceScope)
    {
        var zoneRepository = serviceScope.ServiceProvider.GetService<IZoneRepository>();
        var thisZone = await zoneRepository.GetAsync(ZoneId);

        NextExecutionTime = null;
        
        if (
            thisZone.Owner != null
            && thisZone.Owner.GuildId.HasValue
            && thisZone.Owner.DefendSchedulePostChannel.HasValue
            && thisZone.Owner.DefendBroadcastLeadTime.HasValue)
        {
            thisZone.SetNextDefend();
            if (thisZone.NextDefend.HasValue)
            {
                var nextDefend = thisZone.NextDefend.Value;
                NextExecutionTime = nextDefend.AddMinutes(-1 * thisZone.Owner.DefendBroadcastLeadTime.Value);
                if (NextExecutionTime < DateTime.Now)
                    NextExecutionTime = NextExecutionTime.Value.AddDays(7);
            }
        }
    }

    // TODO: Schedule currently printing twice (ie. at startup, esp if defense is within configured buffer)
    // TODO: The next execution isn't getting scheduled correctly.
    public override async Task DoWork(CancellationToken cancellationToken)
    {
        if (ZoneId == 0) throw new NullReferenceException("Zone has not been set.");
            
        using var thisServiceScope = _serviceProvider.CreateScope();
        _allianceRepository = thisServiceScope.ServiceProvider.GetService<IAllianceRepository>();
        _zoneRepository = thisServiceScope.ServiceProvider.GetService<IZoneRepository>();
        _scheduleResponse = thisServiceScope.ServiceProvider.GetService<Responses.Schedule>();

        var zone = await _zoneRepository.GetAsync(ZoneId);
        if (zone == null)
        {
            _logger.LogWarning($"Unable to post zone reminder for {ZoneId} - Zone not found");
            NextExecutionTime = null;
            return;
        }
        
        var alliance = zone.Owner;
        if (alliance == null)
        {
            _logger.LogWarning($"Unable to post zone reminder for {ZoneId} - Zone not owned");
            NextExecutionTime = null;
            return;
        }
        
        if (!alliance.GuildId.HasValue || !alliance.DefendSchedulePostChannel.HasValue)
        {
            _logger.LogWarning($"Unable to post zone reminder for {ZoneId} for {alliance.Acronym} - Guild or channel cannot be blank");
            return;
        }
            
        try
        {
            var guild = _client.GetGuild(alliance.GuildId.Value);
            if (guild == null)
            {
                _logger.LogError(
                    $"Unable to post zone reminder {ZoneId} to guild {alliance.GuildId.Value} for {alliance.Acronym} - Guild not found");
                NextExecutionTime = null;
                return;
            }
            else
            {
                var channel = guild.GetTextChannel(alliance.DefendSchedulePostChannel.Value);
                if (channel == null)
                {
                    _logger.LogError(
                        $"Unable to post zone reminder {ZoneId} to guild {alliance.GuildId.Value} channel {alliance.DefendSchedulePostChannel.Value} for {alliance.Acronym} - Guild or channel not found");
                    NextExecutionTime = null;
                    return;
                }
                
                var everyoneId = guild.EveryoneRole.Id;
                var reminder =
                    $"@everyone Reminder: Defend for {zone.Name} - <t:{zone.NextDefend.Value.ToUniversalTime().ToUnixTimestamp()}:R>";
                
                await channel.SendMessageAsync(reminder, allowedMentions: AllowedMentions.All);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                $"An unexpected error occurred while trying to post the zone reminder for {ZoneId}");
            NextExecutionTime = null;
            return;
        }

        await SetNextExecutionTime(thisServiceScope);
    }
}