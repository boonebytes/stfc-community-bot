using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using DiscordBot.Domain.Entities.Alliances;
using DiscordBot.Domain.Entities.Zones;
using DiscordBot.Domain.Shared;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace DiscordBot
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly Models.Config.Discord _discordConfig;
        private readonly DiscordSocketClient _client;

        public Worker(ILogger<Worker> logger, Models.Config.Discord discordConfig, IServiceProvider serviceProvider, DiscordSocketClient discordSocketClient)
        {
            _logger = logger;
            _discordConfig = discordConfig;
            _serviceProvider = serviceProvider;
            _client = discordSocketClient;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using (var thisServiceScope = _serviceProvider.CreateScope())
            {
                IAllianceRepository allianceRepository = thisServiceScope.ServiceProvider.GetService<IAllianceRepository>();
                await allianceRepository.InitPostSchedule();

                IZoneRepository zoneRepository = thisServiceScope.ServiceProvider.GetService<IZoneRepository>();
                await zoneRepository.InitZones();
            }
            
            var config = new DiscordSocketConfig { MessageCacheSize = 100 };
            var cmdService = _serviceProvider.GetRequiredService<CommandService>();

            _client.Log += LogAsync;
            cmdService.Log += LogAsync;

            await _client.LoginAsync(TokenType.Bot, _discordConfig.Token);
            await _client.StartAsync();

            _client.Ready += () =>
            {
                _logger.LogInformation("Bot is connected");

                Task.Run(async () =>
                    {
                        var cmdScheduler = _serviceProvider.GetService<Scheduler>();
                        await cmdScheduler.Run(stoppingToken, _discordConfig.SchedulePollSeconds);
                    }
                );

                if (!string.IsNullOrEmpty(_discordConfig.WatchingStatus))
                {
                    Task.Run(async () =>
                        {
                            await _client.SetGameAsync(name: _discordConfig.WatchingStatus, type: ActivityType.Watching);
                        }, stoppingToken);
                }

                return Task.CompletedTask;
            };

            var cmdHandler = _serviceProvider.GetRequiredService<Services.CommandHandler>();
            await cmdHandler.InstallCommandsAsync();
            
            await Task.Delay(-1, stoppingToken);

            await _client.StopAsync();
            await _client.LogoutAsync();
        }

        private Task LogAsync(LogMessage logMessage)
        {
            var level = logMessage.Severity switch
            {
                LogSeverity.Critical => LogLevel.Critical,
                LogSeverity.Error => LogLevel.Error,
                LogSeverity.Warning => LogLevel.Warning,
                LogSeverity.Info => LogLevel.Information,
                LogSeverity.Verbose => LogLevel.Debug,
                LogSeverity.Debug => LogLevel.Trace,
                _ => LogLevel.Warning
            };
            _logger.Log(level, logMessage.ToString());
            return Task.CompletedTask;
        }
    }
}