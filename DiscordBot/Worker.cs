/*
Copyright 2022 Boonebytes

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/

using Discord;
using Discord.Commands;
using Discord.WebSocket;
using DiscordBot.Domain.Entities.Alliances;
using DiscordBot.Domain.Entities.Zones;
using DiscordBot.Models.Config;
using Microsoft.Extensions.Hosting;

namespace DiscordBot;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly Models.Config.Discord _discordConfig;
    private readonly Models.Config.App _appConfig;
    private readonly DiscordSocketClient _client;

    private bool _appInitialized = false;

    public Worker(ILogger<Worker> logger, Models.Config.Discord discordConfig, IServiceProvider serviceProvider, DiscordSocketClient discordSocketClient, App appConfig)
    {
        _logger = logger;
        _discordConfig = discordConfig;
        _serviceProvider = serviceProvider;
        _client = discordSocketClient;
        _appConfig = appConfig;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        /*
        TaskScheduler.UnobservedTaskException += (s, e) =>
        {
            _logger.LogError(e.Exception, "Unobserved Task Exception");
            //Console.WriteLine($"Unobserved Task Exception : {e.Exception.Message}");
            //to actually observe the task, uncomment the below line of code
            //e.SetObserved();
        };
        */
        
        if (_appConfig.RunInit && !_appConfig.RunScheduler)
        {
            using var thisServiceScope = _serviceProvider.CreateScope();
            IAllianceRepository allianceRepository =
                thisServiceScope.ServiceProvider.GetService<IAllianceRepository>();
            await allianceRepository.InitPostSchedule();

            IZoneRepository zoneRepository = thisServiceScope.ServiceProvider.GetService<IZoneRepository>();
            await zoneRepository.InitZones();
        }
        
        var cmdService = _serviceProvider.GetRequiredService<CommandService>();
        var cmdHandler = _serviceProvider.GetRequiredService<Services.CommandHandler>();
        var intHandler = _serviceProvider.GetRequiredService<Services.InteractionHandler>();

        _client.Log += LogAsync;
        cmdService.Log += LogAsync;
        

        await _client.LoginAsync(TokenType.Bot, _discordConfig.Token);
        await _client.StartAsync();

        _client.Ready += () =>
        {
            _logger.LogInformation("Bot is connected");

            if (_appInitialized) return Task.CompletedTask;
            
            _appInitialized = true;
            Task.Run(async () =>
            {
                try
                {
                    Common.DiscordOwner = (await _client.GetApplicationInfoAsync().ConfigureAwait(false)).Owner;
                    if (_appConfig.RunScheduler)
                    {
                        var cmdScheduler = _serviceProvider.GetService<Scheduler>();
                        _ = Task.Run(async () =>
                        {
                            await cmdScheduler.Run(stoppingToken);
                        }, stoppingToken);
                    }
                    
                    
                    await cmdHandler.InstallCommandsAsync();
                    _logger.LogInformation("Command Handler started");
                    
                    await intHandler.InstallCommandsAsync();
                    _logger.LogInformation("Interaction Handler started");
                    
                    if (!string.IsNullOrEmpty(_discordConfig.WatchingStatus))
                        await _client.SetGameAsync(name: _discordConfig.WatchingStatus, type: ActivityType.Watching);
                    _logger.LogInformation("Bot startup complete");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Exception on startup");
                }
            }, stoppingToken);

            return Task.CompletedTask;
        };

        //await Task.Delay(10000);
        //GC.Collect(2);
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