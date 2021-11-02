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
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace DiscordBot
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly Models.Config.Discord _discordConfg;
        private readonly DiscordSocketClient _client;

        public Worker(ILogger<Worker> logger, Models.Config.Discord discordConfig, IServiceProvider serviceProvider, DiscordSocketClient discordSocketClient)
        {
            _logger = logger;
            _discordConfg = discordConfig;
            _serviceProvider = serviceProvider;
            _client = discordSocketClient;
            
            //Console.WriteLine(defendTimes.Zones.Count);
            //Console.WriteLine(defendTimes.Zones.First().NextDefend);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {

            using (var thisServiceScope = _serviceProvider.CreateScope())
            {
                IAllianceRepository allianceRepository = thisServiceScope.ServiceProvider.GetService<IAllianceRepository>();
                var initSchedule = allianceRepository.InitPostSchedule();

                IZoneRepository zoneRepository = thisServiceScope.ServiceProvider.GetService<IZoneRepository>();
                var initZones = zoneRepository.InitZones();

                Task.WaitAll(initSchedule, initZones);
            }

            var _config = new DiscordSocketConfig { MessageCacheSize = 100 };
            var cmdService = _serviceProvider.GetRequiredService<CommandService>();

            //_client = new DiscordSocketClient();

            _client.Log += LogAsync;
            cmdService.Log += LogAsync;

            //_client.MessageReceived += ClientOnMessageReceived;

            await _client.LoginAsync(TokenType.Bot, _discordConfg.Token);
            await _client.StartAsync();

            //_client.MessageUpdated += MessageUpdated;
            _client.Ready += () =>
            {
                _logger.LogInformation("Bot is connected");
                Task.Run(async () =>
                    {
                        var cmdScheduler = _serviceProvider.GetService<Scheduler>();
                        await cmdScheduler.Run(stoppingToken, _discordConfg.SchedulePollSeconds);
                    }
                );

                if (!string.IsNullOrEmpty(_discordConfg.WatchingStatus))
                {
                    Task.Run(async () =>
                        {
                            await _client.SetGameAsync(name: _discordConfg.WatchingStatus, type: ActivityType.Watching);
                        }
                    );
                }
                // await _client.SetGameAsync(name: "Star Trek Fleet Command", type: ActivityType.Watching);

                return Task.CompletedTask;
            };

            var cmdHandler = _serviceProvider.GetRequiredService<Services.CommandHandler>();
            await cmdHandler.InstallCommandsAsync();

            //await Task.Delay(1000, stoppingToken);
            await Task.Delay(-1, stoppingToken);

            await _client.StopAsync();
            await _client.LogoutAsync();

            //}
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

        private Task ClientOnMessageReceived(SocketMessage arg)
        {
            if (arg.Content.StartsWith("!helloworld"))
            {
                arg.Channel.SendMessageAsync($"User '{arg.Author.Username}' successfully ran helloworld!");
            }
            return Task.CompletedTask;
        }

        private async Task MessageUpdated(Cacheable<IMessage, ulong> before, SocketMessage after, ISocketMessageChannel channel)
        {
            // If the message was not in the cache, downloading it will result in getting a copy of `after`.
            var message = await before.GetOrDownloadAsync();
            _logger.LogDebug($"{message} -> {after}");
        }
    }
}