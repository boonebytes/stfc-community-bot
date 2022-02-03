using System.Text;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using DiscordBot.Domain.Entities.Alliances;
using DiscordBot.Domain.Entities.Zones;
using DiscordBot.Domain.Shared;

namespace DiscordBot.Responses;

public class Schedule
{
    private readonly ILogger<Schedule> _logger;
    private readonly IZoneRepository _zoneRepository;
    private readonly DiscordSocketClient _client;

    public Schedule(ILogger<Schedule> logger, IZoneRepository zoneRepository, DiscordSocketClient client)
    {
        _logger = logger;
        _zoneRepository = zoneRepository;
        _client = client;
    }

    public string GetDiscordEmbedTitle(Zone zone)
    {
        if (zone.Owner == null)
            return $"Unclaimed - {zone.Name} ({zone.Level}^)"; 
        else
            return $"{zone.Owner.Acronym} - {zone.Name} ({zone.Level}^)";
    }

    public string GetDiscordEmbedValue(Zone zone, bool shortVersion = false, bool useNextWeek = false)
    {
        var response = new StringBuilder();
        //var tz = TimeZoneInfo.ConvertTime(NextDefend.Value, )

        string potentialThreats = "";
        var potentialHostiles = _zoneRepository
            .GetPotentialHostiles(zone.Id)
            .Select(a => a.Acronym)
            .OrderBy(a => a);

        potentialThreats = string.Join(", ", potentialHostiles);

        if (shortVersion)
        {
            response.Append($"{zone.Owner.Acronym}/{zone.Name}({zone.Level}^): <t:");
            if (useNextWeek)
            {
                response.Append(zone.NextDefend.Value.ToUniversalTime().AddDays(7).ToUnixTimestamp());
            }
            else
            {
                response.Append(zone.NextDefend.Value.ToUniversalTime().ToUnixTimestamp());
            }
            response.Append($":t> local / {zone.NextDefend.Value.ToEasternTime().ToString("h:mm tt")} ET");

            //if (!string.IsNullOrEmpty(zone.Threats))
            //    response += " [*_" + zone.Threats + "_*]";
            if (!string.IsNullOrEmpty(potentialThreats))
                response.Append(" [*_" + potentialThreats + "_*]");
            else if (zone.LowRisk)
                response.Append(" [*_Low Risk_*]");
        }
        else
        {
            if (string.IsNullOrEmpty(potentialThreats) && string.IsNullOrEmpty(zone.Threats) && zone.LowRisk)
                response.Append("*_Low Risk_*\n");

            response.Append($"**When**: <t:");
            if (useNextWeek)
            {
                response.Append(zone.NextDefend.Value.ToUniversalTime().AddDays(7).ToUnixTimestamp());
            }
            else
            {
                response.Append(zone.NextDefend.Value.ToUniversalTime().ToUnixTimestamp());
            }
            response.Append(":t> local / "
                            + $"{zone.DefendUtcTime} UTC / "
                            + $"{zone.NextDefend.Value.ToEasternTime().ToString("h:mm tt")} ET");
            //if (!string.IsNullOrEmpty(zone.Threats))
            //    response += "\n**Saved Threats**: " + zone.Threats;
            response.Append("\n**Nearby Threats**: " + (string.IsNullOrEmpty(potentialThreats) ? "None" : potentialThreats));
            if (!string.IsNullOrEmpty(zone.Notes))
            {
                response.Append($"\n**Notes**: {zone.Notes}");
            }
        }

        return response.ToString();
    }


    public string GetDayScheduleAsString(List<Zone> zones, DayOfWeek day, bool includeDayHeader = true)
    {
        string indent = "";
        var response = new StringBuilder();

        var dayZones = zones.Where(z => z.DefendEasternDay == day)
            .ToList();

        if (includeDayHeader)
        {
            response.AppendLine("**__" + day.ToString() + "__**");
            indent = "> ";
        }

        if (!dayZones.Any())
        {
            response.AppendLine(indent + "(empty)");
            return response.ToString();
        }

        foreach (Zone zone in dayZones)
        {
            response.AppendLine(indent + GetDiscordEmbedValue(zone, true));
        }
        return response.ToString();
    }
        
    protected async Task PostDefendsViaTextAsync(IMessageChannel channel, List<Zone> zones)
    {
        //string nextMessage = "";

        bool includeDayHeaders = zones.Select(z => z.DefendEasternDay).Distinct().Count() > 1;

        for (var i = 0; i < 7; i++)
        {
            DayOfWeek day = (DayOfWeek)i;
            var postMessage = GetDayScheduleAsString(zones, day, includeDayHeaders);
            await channel.SendMessageAsync(postMessage);
        }
    }

    protected async Task PostDefendsViaEmbedsAsync(SocketCommandContext context, string title, List<Zone> zones)
    {
        if (!zones.Any())
        {
            var embedMsg = new EmbedBuilder
            {
                Title = title
                //Description = ""
            };

            var thisField = new EmbedFieldBuilder
            {
                Name = "None Scheduled",
                Value = "There are no defends scheduled."
            };
            embedMsg.AddField(thisField);
            await context.Channel.SendMessageAsync(embed: embedMsg.Build());
        }
        else
        {
            bool includeDayHeaders = zones.Select(z => z.DefendEasternDay).Distinct().Count() > 1;

            var embedMsg = new EmbedBuilder
            {
                Title = title
                //Description = ""
            };
            DayOfWeek? lastDay = null;
            foreach (Zone zone in zones)
            {
                bool useNextWeek =
                    includeDayHeaders
                    && zone.DefendEasternDay >= DateTime.Now.ToEasternTime().DayOfWeek
                    && zone.DefendEasternDay != DayOfWeek.Sunday;

                if (includeDayHeaders && (!lastDay.HasValue || lastDay.Value != zone.DefendEasternDay))
                {
                    var fieldHeader = new EmbedFieldBuilder
                    {
                        Name = "*__" + zone.DefendEasternDay.ToString() + "__*",
                        Value = "\u200B"
                    };
                    embedMsg.AddField(fieldHeader);
                    lastDay = zone.DefendEasternDay;
                }

                var thisField = new EmbedFieldBuilder
                {
                    Name = GetDiscordEmbedTitle(zone),
                    Value = GetDiscordEmbedValue(zone, false, useNextWeek) + "\n\u200b"
                };
                embedMsg.AddField(thisField);
            }
        }

    }

    protected void AddDefendsToEmbed(List<Zone> zones, ref EmbedBuilder embedMsg, bool shortVersion = false)
    {
        if (!zones.Any())
        {
            var thisField = new EmbedFieldBuilder
            {
                Name = "None Scheduled",
                Value = "There are no defends scheduled."
            };
            embedMsg.AddField(thisField);
        }
        else
        {
            bool includeDayHeaders = zones.Select(z => z.DefendEasternDay).Distinct().Count() > 1;

            DayOfWeek? lastDay = null;
            int currentLine = 0;
            foreach (Zone zone in zones)
            {
                bool useNextWeek =
                    includeDayHeaders
                    && zone.DefendEasternDay >= DateTime.Now.ToEasternTime().DayOfWeek
                    && zone.DefendEasternDay != DayOfWeek.Sunday;

                if (shortVersion)
                {
                    if (includeDayHeaders)
                    {
                        if (!lastDay.HasValue || lastDay.Value != zone.DefendEasternDay)
                        {
                            if (currentLine > 0) embedMsg.Description += "\n";
                            embedMsg.Description += "**__" + zone.DefendEasternDay.ToString() + "__**" + "\n";
                            lastDay = zone.DefendEasternDay;
                        }
                        embedMsg.Description += "> ";
                    }
                    embedMsg.Description += GetDiscordEmbedValue(zone, true, useNextWeek) + "\n";
                }
                else
                {
                    if (includeDayHeaders && (!lastDay.HasValue || lastDay.Value != zone.DefendEasternDay))
                    {
                        var fieldHeader = new EmbedFieldBuilder
                        {
                            Name = "*__" + zone.DefendEasternDay.ToString() + "__*",
                            Value = "\u200B"
                        };
                        embedMsg.AddField(fieldHeader);
                        lastDay = zone.DefendEasternDay;
                    }
                        
                    var thisField = new EmbedFieldBuilder
                    {
                        Name = GetDiscordEmbedTitle(zone),
                        Value = GetDiscordEmbedValue(zone, false, useNextWeek) + "\n\u200b"
                    };
                    embedMsg.AddField(thisField);
                }
                currentLine++;
            }
        }
    }

    public EmbedBuilder GetForDate(DateTime date, long? allianceId = null, bool shortVersion = false)
    {
        var embedMsg = new EmbedBuilder
        {
            Title = "Defend Schedule for " + date.ToString("dddd, MMM d")
        };

        var fromDate = date.ToUniversalTime();
        if (fromDate.Hour < 4)
        {
            fromDate = fromDate.AddDays(-1);
        }
        fromDate = fromDate.AddHours(-date.ToUniversalTime().Hour + 3);

        var todayDefends = _zoneRepository.GetNext24Hours(fromDate, allianceId).OrderBy(z => z.NextDefend).ToList();

        AddDefendsToEmbed(todayDefends, ref embedMsg, shortVersion);

        return embedMsg;
    }

    public EmbedBuilder GetNext(long? allianceId = null)
    {
        var embedMsg = new EmbedBuilder
        {
            Title = "Next Defense"
        };
        var nextDefend = _zoneRepository.GetNextDefend(allianceId);
        if (nextDefend == null)
        {
            embedMsg.Description = "No defends were found.";
            return embedMsg;
        }
        else
        {
            var thisField = new EmbedFieldBuilder
            {
                Name = GetDiscordEmbedTitle(nextDefend),
                Value = GetDiscordEmbedValue(nextDefend) + "\n\u200b"
            };
            embedMsg.AddField(thisField);
            return embedMsg;
        }
    }

    public async Task PostAllAsync(ulong guildId, ulong channelId, long? allianceId = null, bool shortVersion = false)
    {
        try
        {
            var allDefends = (await _zoneRepository.GetAllAsync(allianceId, false))
                .OrderBy(z => z.DefendEasternDay)
                .ThenBy(z => z.DefendEasternTime)
                .ToList();

            var channel = _client.GetGuild(guildId).GetTextChannel(channelId);

            await PostDefendsViaTextAsync(channel, allDefends);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Unexpected error occured while trying to write all to {guildId} {channelId}");
        }
    }



    // SCHEDULER SPECIFIC

    public async Task TryCleanMessages(SocketTextChannel channel, IEnumerable<IMessage> channelMessages, Alliance alliance)
    {
        try
        {
            // Delete any of the long "defend schedule for today" kind of messages
            var myMessages = channelMessages.Where(m =>
                !m.IsPinned
                && m.Author.Id == _client.CurrentUser.Id
                && (DateTimeOffset.UtcNow - m.Timestamp).TotalDays <= 14
                && (
                    (
                        m.Embeds.Count == 1
                        && m.Embeds.First().Title.StartsWith("Defend Schedule for ")
                    )
                    || (
                        m.Embeds.Count == 0
                        && m.MentionedEveryone == true
                    )
                )
            );
            if (myMessages.Any())
            {
                await channel.DeleteMessagesAsync(myMessages.ToList());
            }
                
            // Delete pinned notifications
            var pinnedNotifications = channelMessages.Where(m =>
                    m.Author.Id == _client.CurrentUser.Id
                    && m.Type == MessageType.ChannelPinnedMessage
                    && m.Timestamp > DateTime.Now.AddDays(-10)
                )
                .ToList();
            if (pinnedNotifications.Any())
            {
                foreach (Discord.Rest.RestSystemMessage msg in pinnedNotifications)
                {
                    await msg.DeleteAsync();
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, $"Unable to delete messages for new schedule for {alliance.Acronym} in guild {alliance.GuildId.Value} channel {alliance.DefendSchedulePostChannel.Value}");
        }
    }

    public async Task TryUpdateWeeklyMessages(IEnumerable<IMessage> channelMessages, Alliance alliance)
    {
        try
        {
            DayOfWeek currentDay = DayOfWeek.Sunday;
            while (currentDay <= DayOfWeek.Saturday)
            {
                await TryUpdateDayMessage(channelMessages, alliance, currentDay);
                currentDay += 1;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Unable to update all weekly messages for {alliance.Acronym}");
        }
    }

    public async Task TryUpdateDayMessage(IEnumerable<IMessage> channelMessages, Alliance alliance, DayOfWeek dayOfWeek)
    {
        try
        {
            var dayShortPosts = channelMessages.Where(m =>
                    m.Author.Id == _client.CurrentUser.Id
                    && m.Embeds.Count == 0
                    && m.Content.StartsWith("**__" + dayOfWeek.ToString() + "__**")
                )
                .ToList();

            if (dayShortPosts.Count == 1)
            {
                var dayShortPost = (Discord.Rest.RestUserMessage)dayShortPosts.First();
                if (dayShortPost.IsPinned && dayOfWeek != DateTime.Now.ToEasternTime().DayOfWeek)
                    await dayShortPost.UnpinAsync();

                var dayDefends = _zoneRepository.GetFromDayOfWeek(
                        dayOfWeek,
                        alliance.Id)
                    .OrderBy(z => z.DefendEasternTime)
                    .ToList();
                await dayShortPost.ModifyAsync(msg =>
                    msg.Content =
                        this.GetDayScheduleAsString(dayDefends, dayOfWeek, true)
                        + $"*_Last Updated: <t:{DateTime.UtcNow.ToUnixTimestamp()}:R>_*" + "\n\u200b"
                );

            }
            else
            {
                _logger.LogError($"Unable to find mesage to edit {dayOfWeek} day-of-week schedule for {alliance.Acronym}. Records returned: {dayShortPosts.Count}");
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, $"Unable to edit message for {dayOfWeek} day-of-week schedule for {alliance.Acronym}.");
        }
    }
}