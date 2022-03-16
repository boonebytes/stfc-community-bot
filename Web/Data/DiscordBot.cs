using Discord;
using Discord.WebSocket;

namespace Web.Data;

public class DiscordBot : IDisposable
{
    private readonly ILogger<DiscordBot> _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly Models.Config.Discord _discordConfig;
    private readonly DiscordSocketClient _client;

    private bool _hasLoggedIn { get; set; } = false;
    
    public DiscordBot(ILogger<DiscordBot> logger, IServiceProvider serviceProvider, Models.Config.Discord discordConfig, DiscordSocketClient client)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
        _discordConfig = discordConfig;
        _client = client;
    }

    public async Task StartAsync()
    {
        await _client.LoginAsync(TokenType.Bot, _discordConfig.Token);
        await _client.StartAsync();

        _client.Ready += () => {
            _logger.LogInformation("Bot is connected");
            _hasLoggedIn = true;
            return Task.CompletedTask;
        };
    }

    public void Dispose()
    {
        //_client.LogoutAsync();
        _client.Dispose();
    }
}