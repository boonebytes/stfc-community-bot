using System.Reflection;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using DiscordBot.Domain.Entities.Admin;

namespace DiscordBot.Services;

public class CommandHandler
{
    private readonly ILogger<CommandHandler> _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly IServiceProvider _scopedProvider;
    private readonly DiscordSocketClient _client;
    private readonly CommandService _commands;

    private IDMChannel _ownerChannel;

    // Retrieve client and CommandService instance via ctor
    public CommandHandler(ILogger<CommandHandler> logger, DiscordSocketClient client, CommandService commands, IServiceProvider serviceProvider)
    {
        _logger = logger;
        _commands = commands;
        _client = client;
        _serviceProvider = serviceProvider;

        var serviceScope = _serviceProvider.CreateScope();
        _scopedProvider = serviceScope.ServiceProvider;
    }

    public async Task InstallCommandsAsync()
    {
        try
        {
            _commands.Log += LogCommandMessage;
            _ownerChannel = await Common.DiscordOwner.CreateDMChannelAsync();

            // Hook the MessageReceived event into our command handler
            _client.MessageReceived += HandleCommandAsync;

            // Here we discover all of the command modules in the entry 
            // assembly and load them. Starting from Discord.NET 2.0, a
            // service provider is required to be passed into the
            // module registration method to inject the 
            // required dependencies.
            //
            // If you do not use Dependency Injection, pass null.
            // See Dependency Injection guide for more information.

            /*
            using (var scope = _serviceProvider.CreateScope())
            {
                await _commands.AddModulesAsync(assembly: Assembly.GetEntryAssembly(),
                                                services: scope.ServiceProvider);
            }
            */
            
            await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), _scopedProvider);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception occured while starting the command handler");
        }
    }

    private Task LogCommandMessage(LogMessage logMessage)
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

    private async Task HandleCommandAsync(SocketMessage messageParam)
    {
        // Don't process the command if it was a system message
        var message = messageParam as SocketUserMessage;
        if (message == null) return;

        if (message.Author.IsBot) return;

        
        //// Create a number to track where the prefix ends and the command begins
        //int argPos = 0;
        //
        //// Determine if the message is a command based on the prefix and make sure no bots trigger commands
        //if (
        //    message.HasStringPrefix(_discordConfig.Prefix, ref argPos)
        //    || message.HasMentionPrefix(_client.CurrentUser, ref argPos)
        //)
        //{
        //    // Create a WebSocket-based command context based on the message
        //    var context = new SocketCommandContext(_client, message);
        //
        //    // Execute the command with the command context we just
        //    // created, along with the service provider for precondition checks.
        //        
        //    // Initially developed to create the scope before calling the
        //    // Discord modules. Some commands needed to run async due
        //    // to the Discord gateway's response timeout. This caused
        //    // issues with the scope closing before the threads finished,
        //    // which would then release the EFCore contexts, throw errors,
        //    // etc. Because of this, the scope was moved from outside the
        //    // thread to inside the thread.
        //    /*
        //    using (var scope = _serviceProvider.CreateScope())
        //    {
        //        await _commands.ExecuteAsync(
        //            context: context,
        //            argPos: argPos,
        //            services: scope.ServiceProvider);
        //    }
        //    */
        //    await _commands.ExecuteAsync(
        //        context: context,
        //        argPos: argPos,
        //        services: _scopedProvider);
        //}
        //else
        //{
        if (message.Channel is SocketDMChannel dmChannel)
        {
            // Log any direct messages to the database.
            _ = Task.Run(async() => {
                try
                {
                    using var scope = _serviceProvider.CreateScope();
                    var dmRepository = scope.ServiceProvider.GetService<IDirectMessageRepository>();
                    var localClient = scope.ServiceProvider.GetService<DiscordSocketClient>();
                    var submittedBy = localClient.GetUser(message.Author.Id);

                    var dm = new DirectMessage(message.Author.Id, message.Content);
                    foreach (var commonServer in submittedBy.MutualGuilds)
                    {
                        dm.AddServer(commonServer.Id, commonServer.Name);
                    }

                    dmRepository.Add(dm);
                    await dmRepository.UnitOfWork.SaveEntitiesAsync();
                    await dmChannel.SendMessageAsync("Thank you; your message has been received!");

                    await _ownerChannel.SendMessageAsync(
                        "------------------------\n"
                        + "New private message received\n"
                        + "Servers: " + dm.CommonServers + "\n"
                        + "------------------------\n"
                    );
                    await _ownerChannel.SendMessageAsync(message.Content);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Unable to process DM");
                }
            });
            await Task.Delay(500);
            //}
        }
    }
}