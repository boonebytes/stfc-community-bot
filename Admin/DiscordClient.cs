using System;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace DiscordBot.AdminWeb
{
    public class DiscordClient : BackgroundService
    {
        private readonly ILogger<DiscordClient> _logger;
        private readonly Models.Config.Discord _discordConfg;

        private readonly DiscordSocketClient _client;
        public DiscordSocketClient Client
        {
            get
            {
                return _client;
            }
        }

        public DiscordClient(ILogger<DiscordClient> logger, Models.Config.Discord discordConfig, DiscordSocketClient discordSocketClient)
        {
            _logger = logger;
            _discordConfg = discordConfig;
            _client = discordSocketClient;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var _config = new DiscordSocketConfig { MessageCacheSize = 100 };
            _client.Log += LogAsync;
            await _client.LoginAsync(TokenType.Bot, _discordConfg.Token);
            await _client.StartAsync();

            _client.Ready += () =>
            {
                _logger.LogInformation("Bot is connected");


                return Task.CompletedTask;
            };

            await Task.Delay(-1, stoppingToken);

            await _client.StopAsync();
            await _client.LogoutAsync();

        }

        private Task LogAsync(LogMessage logMessage)
        {
            LogLevel level = LogLevel.Warning;
            switch (logMessage.Severity)
            {
                case LogSeverity.Critical:
                    level = LogLevel.Critical;
                    break;
                case LogSeverity.Error:
                    level = LogLevel.Error;
                    break;
                case LogSeverity.Warning:
                    level = LogLevel.Warning;
                    break;
                case LogSeverity.Info:
                    level = LogLevel.Information;
                    break;
                case LogSeverity.Verbose:
                    level = LogLevel.Debug;
                    break;
                case LogSeverity.Debug:
                    level = LogLevel.Trace;
                    break;
            }
            _logger.Log(level, logMessage.ToString());
            return Task.CompletedTask;
        }
    }
}
