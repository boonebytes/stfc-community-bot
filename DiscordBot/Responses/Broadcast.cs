using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;

namespace DiscordBot.Responses
{
    public class Broadcast
    {
        private readonly ILogger<Broadcast> _logger;
        private readonly Managers.DiscordServers _discordServers;
        private readonly DiscordSocketClient _client;

        public Broadcast(ILogger<Broadcast> logger, Managers.DiscordServers discordServers, DiscordSocketClient client)
        {
            _logger = logger;
            _discordServers = discordServers;
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

            foreach (Models.Tdl.DiscordServer server in _discordServers.Servers)
            {
                var targetGuild = _client.GetGuild(server.GuildId);
                if (targetGuild == null)
                {
                    _logger.LogError($"Unable to broadcast for {fromGuildUser.Nickname} ({fromGuildUser.Id}) from {fromGuildUser.Guild.Name} ({fromGuildUser.Guild.Id}) to {server.AllianceAcronym} ({server.GuildId}) - Guild could not be resolved.");
                }
                else
                {
                    var targetChannel = targetGuild.GetTextChannel(server.PostToChannel);
                    if (targetChannel == null)
                    {
                        _logger.LogError($"Unable to broadcast for {fromGuildUser.Nickname} ({fromGuildUser.Id}) from {fromGuildUser.Guild.Name} ({fromGuildUser.Guild.Id}) to {server.AllianceAcronym} ({server.GuildId}) {server.PostToChannel} - Channel could not be resolved.");
                    }
                    else
                    {
                        _logger.LogInformation($"Sending broadcast for {fromGuildUser.Nickname} ({fromGuildUser.Id}) from {fromGuildUser.Guild.Name} ({fromGuildUser.Guild.Id}) to {server.AllianceAcronym} ({server.GuildId}) {targetChannel.Name} ({server.PostToChannel})");
                        await targetChannel.SendMessageAsync(embed: embedMsg);
                        sentTo += 1;
                    }
                }
            }
            await fromTextChannel.SendMessageAsync($"Broadcast sent to {sentTo} other Discord servers.");
        }

    }
}
