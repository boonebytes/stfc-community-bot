using Discord;
using Discord.WebSocket;
using DiscordBot.Domain.Entities.Alliances;
using DiscordBot.Domain.Entities.Zones;
using DiscordBot.Domain.Shared;
using Quartz;

namespace DiscordBot.Jobs;

[DisallowConcurrentExecution]
public class PostDefendReminder : BaseJob
{
    private readonly DiscordSocketClient _client;
    
    private IAllianceRepository _allianceRepository;
    private IZoneRepository _zoneRepository;
    private Responses.Schedule _scheduleResponse;

    public PostDefendReminder(
        ILogger<PostDefendReminder> logger,
        DiscordSocketClient client,
        IAllianceRepository allianceRepository,
        IZoneRepository zoneRepository,
        Responses.Schedule scheduleResponse
        ) : base(logger)
    {
        _client = client;
        _allianceRepository = allianceRepository;
        _zoneRepository = zoneRepository;
        _scheduleResponse = scheduleResponse;
    }
    
    protected override async Task DoWork(IJobExecutionContext context)
    {
        var data = context.JobDetail.JobDataMap;
        var allianceId = data.GetLong("allianceId");
        var zoneId = data.GetLong("itemId");

        if (allianceId == 0)
            throw new NullReferenceException("Alliance has not been set");
        if (zoneId == 0)
            throw new NullReferenceException("Zone has not been set");

        var alliance = await _allianceRepository.GetAsync(allianceId);
        if (alliance == null)
            throw new KeyNotFoundException("The alliance could not be found");
        if (!alliance.GuildId.HasValue || !alliance.DefendSchedulePostChannel.HasValue)
            throw new InvalidOperationException("Alliance doesn't have a guild and/or defend channel");

        var zone = await _zoneRepository.GetAsync(zoneId);
        if (zone == null)
            throw new KeyNotFoundException("The zone could not be found");
        if (zone.Owner != alliance)
            throw new InvalidOperationException("The zone is not owned by this alliance");

        var guild = _client.GetGuild(alliance.GuildId.Value);
        if (guild == null)
            throw new InvalidOperationException("Unable to access guild");
        
        var channel = guild.GetTextChannel(alliance.DefendSchedulePostChannel.Value);
        if (channel == null)
            throw new InvalidOperationException("Unable to access channel");

        var rolePing = "";
        if (!zone.LowRisk || (alliance.DefendBroadcastPingForLowRisk.HasValue &&
                              alliance.DefendBroadcastPingForLowRisk.Value))
        {
            if (alliance.DefendBroadcastPingRole.HasValue)
            {
                if (alliance.DefendBroadcastPingRole.Value == ulong.MaxValue)
                {
                    rolePing = "@everyone ";
                }
                else
                {
                    rolePing = $"<@&{alliance.DefendBroadcastPingRole.Value}> ";
                }
            }
        }

        var reminder =
            $"{rolePing}Reminder: Defend for {zone.Name} - <t:{zone.NextDefend.Value.ToUniversalTime().ToUnixTimestamp()}:R>";

        //var embedBuilder = new EmbedBuilder
        //{
        //    Color = Color.Purple,
        //    Title = $"Defend for {zone.Name}",
        //    Description = reminder
        //};
        //await channel.SendMessageAsync(embed: embedBuilder.Build(), allowedMentions: AllowedMentions.All);
        await channel.SendMessageAsync(reminder, allowedMentions: AllowedMentions.All);
    }
}