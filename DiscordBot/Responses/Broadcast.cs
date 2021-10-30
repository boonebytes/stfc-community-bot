using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using DiscordBot.Domain.Entities.Alliances;
using Microsoft.Extensions.Logging;

namespace DiscordBot.Responses
{
    public class Broadcast
    {
        private readonly ILogger<Broadcast> _logger;
        private readonly IAllianceRepository _allianceRepository;
        private readonly DiscordSocketClient _client;

        public Broadcast(ILogger<Broadcast> logger, IAllianceRepository allianceRepository, DiscordSocketClient client)
        {
            _logger = logger;
            _allianceRepository = allianceRepository;
            _client = client;
        }

        public async Task SendBroadcast(ISocketMessageChannel fromTextChannel, SocketGuildUser fromGuildUser, string message)
        {
            EmbedBuilder embedBuilder = new EmbedBuilder
            {
                Title = $"Broadcast Message from {fromGuildUser.Guild.Name}",
                Description = message
            };
            embedBuilder.WithAuthor(fromGuildUser.Nickname);

            var embedMsg = embedBuilder.Build();

            int sentTo = 0;

            foreach (Alliance alliance in _allianceRepository.GetAllWithServers())
            {
                var targetGuild = _client.GetGuild(alliance.GuildId.Value);
                if (targetGuild == null)
                {
                    _logger.LogError($"Unable to broadcast for {fromGuildUser.Nickname} ({fromGuildUser.Id}) from {fromGuildUser.Guild.Name} ({fromGuildUser.Guild.Id}) to {alliance.Acronym} ({alliance.GuildId.Value}) - Guild could not be resolved.");
                }
                else
                {
                    var targetChannel = targetGuild.GetTextChannel(alliance.DefendSchedulePostChannel.Value);
                    if (targetChannel == null)
                    {
                        _logger.LogError($"Unable to broadcast for {fromGuildUser.Nickname} ({fromGuildUser.Id}) from {fromGuildUser.Guild.Name} ({fromGuildUser.Guild.Id}) to {alliance.Acronym} ({alliance.GuildId.Value}) {alliance.DefendSchedulePostChannel} - Channel could not be resolved.");
                    }
                    else
                    {
                        _logger.LogInformation($"Sending broadcast for {fromGuildUser.Nickname} ({fromGuildUser.Id}) from {fromGuildUser.Guild.Name} ({fromGuildUser.Guild.Id}) to {alliance.Acronym} ({alliance.GuildId.Value}) {targetChannel.Name} ({alliance.DefendSchedulePostChannel.Value})");
                        await targetChannel.SendMessageAsync(embed: embedMsg);
                        sentTo += 1;
                    }
                }
            }
            await fromTextChannel.SendMessageAsync($"Broadcast sent to {sentTo} other Discord servers.");
        }

    }
}
