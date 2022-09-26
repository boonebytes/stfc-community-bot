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