﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using DiscordBot.Domain.Entities.Zones;
using DiscordBot.Domain.Shared;
using Microsoft.Extensions.Logging;

namespace DiscordBot.Responses
{
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

        public string GetDayScheduleAsString(List<Zone> zones, DayOfWeek day, bool includeDayHeader = true)
        {
            string indent = "";
            string response = "";

            var dayZones = zones.Where(z => z.DefendEasternDay == day);

            if (includeDayHeader)
            {
                response = "**__" + day.ToString() + "__**" + "\n";
                indent = "> ";
            }

            if (dayZones.Count() == 0)
            {
                response += indent + "(empty)";
                return response;
            }

            foreach (Zone zone in dayZones)
            {
                bool useNextWeek = false;
                if (includeDayHeader && zone.DefendEasternDay >= DateTime.Now.ToEasternTime().DayOfWeek && zone.DefendEasternDay != DayOfWeek.Sunday)
                    useNextWeek = true;

                response += indent + zone.GetDiscordEmbedValue(true, useNextWeek) + "\n";
            }
            return response;
        }
        
        protected async Task PostDefendsViaTextAsync(IMessageChannel channel, List<Zone> zones)
        {
            //string nextMessage = "";

            bool includeDayHeaders = false;
            if (zones.Select(z => z.DefendEasternDay).Distinct().Count() > 1)
                includeDayHeaders = true;

            for (var i = 0; i < 7; i++)
            {
                DayOfWeek day = (DayOfWeek)i;
                var postMessage = GetDayScheduleAsString(zones, day, includeDayHeaders);
                await channel.SendMessageAsync(postMessage);
            }
            /*
            DayOfWeek? lastDay = null;
            foreach (Zone zone in zones)
            {
                bool useNextWeek = false;
                if (includeDayHeaders && zone.DefendEasternDay >= DateTime.Now.ToEasternTime().DayOfWeek && zone.DefendEasternDay != DayOfWeek.Sunday)
                    useNextWeek = true;

                if (includeDayHeaders)
                {
                    if (!lastDay.HasValue || lastDay.Value != zone.DefendEasternDay)
                    {
                        if (nextMessage != "")
                            await channel.SendMessageAsync(nextMessage);
                        DayOfWeek dowIndex = DayOfWeek.Sunday;
                        if (lastDay.HasValue)
                            dowIndex = lastDay.Value + 1;

                        int dowCounter = 0; // Simply to prevent a potential infinite loop
                        while (dowIndex < zone.DefendEasternDay && dowIndex <= DayOfWeek.Saturday && dowCounter < 14)
                        {
                            nextMessage = "**__" + dowIndex.ToString() + "__**" + "\n"
                                        + "> (empty)";
                            await channel.SendMessageAsync(nextMessage);

                            dowCounter++;
                            dowIndex++;
                        }

                        nextMessage = "";
                        nextMessage += "**__" + zone.DefendEasternDay.ToString() + "__**" + "\n";
                        lastDay = zone.DefendEasternDay;
                    }
                    nextMessage += "> ";
                }
                nextMessage += zone.GetDiscordEmbedValue(true, useNextWeek) + "\n";
            }

            if (nextMessage != "")
                await channel.SendMessageAsync(nextMessage);
            */
        }

        protected async Task PostDefendsViaEmbedsAsync(SocketCommandContext context, string title, List<Zone> zones)
        {
            if (zones.Count() == 0)
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
                bool includeDayHeaders = false;
                if (zones.Select(z => z.DefendEasternDay).Distinct().Count() > 1)
                    includeDayHeaders = true;

                var embedMsg = new EmbedBuilder
                {
                    Title = title
                    //Description = ""
                };
                DayOfWeek? lastDay = null;
                int currentLine = 0;
                foreach (Zone zone in zones)
                {
                    bool useNextWeek = false;
                    if (includeDayHeaders && zone.DefendEasternDay >= DateTime.Now.ToEasternTime().DayOfWeek && zone.DefendEasternDay != DayOfWeek.Sunday)
                        useNextWeek = true;

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
                        Name = zone.GetDiscordEmbedName(),
                        Value = zone.GetDiscordEmbedValue(false, useNextWeek) + "\n\u200b"
                    };
                    embedMsg.AddField(thisField);
                    
                    currentLine++;
                }
            }

        }

        protected void AddDefendsToEmbed(List<Zone> zones, ref EmbedBuilder embedMsg, bool shortVersion = false)
        {
            if (zones.Count() == 0)
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
                bool includeDayHeaders = false;
                if (zones.Select(z => z.DefendEasternDay).Distinct().Count() > 1)
                    includeDayHeaders = true;

                DayOfWeek? lastDay = null;
                int currentLine = 0;
                foreach (Zone zone in zones)
                {
                    bool useNextWeek = false;
                    if (includeDayHeaders && zone.DefendEasternDay >= DateTime.Now.ToEasternTime().DayOfWeek && zone.DefendEasternDay != DayOfWeek.Sunday)
                        useNextWeek = true;

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
                        embedMsg.Description += zone.GetDiscordEmbedValue(true, useNextWeek) + "\n";
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
                            Name = zone.GetDiscordEmbedName(),
                            Value = zone.GetDiscordEmbedValue(false, useNextWeek) + "\n\u200b"
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
                //Description = ""
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
                //Description = ""
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
                    Name = nextDefend.GetDiscordEmbedName(),
                    Value = nextDefend.GetDiscordEmbedValue() + "\n\u200b"
                };
                embedMsg.AddField(thisField);
                return embedMsg;
            }
        }

        public async Task PostAllAsync(ulong guildId, ulong channelId, long? allianceId = null, bool shortVersion = false)
        {
            /*
            var embedMsg = new EmbedBuilder
            {
                Title = "Full Defend Schedule"
                //Description = ""
            };
            */

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
            
            //AddDefendsToEmbed(allDefends, ref embedMsg, shortVersion);
        }
    }
}
