using System;
using System.Reflection;
using System.Threading.Tasks;
using Discord.Commands;
using Discord.WebSocket;
using DiscordBot.Domain.Entities.Admin;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace DiscordBot.Services
{
    public class CommandHandler
    {
        private readonly ILogger<CommandHandler> _logger;
        private readonly Models.Config.Discord _discordConfig;
        private readonly IServiceProvider _serviceProvider;
        private readonly IServiceScope _serviceScope;
        private readonly IServiceProvider _scopedProvider;
        private readonly DiscordSocketClient _client;
        private readonly CommandService _commands;

        // Retrieve client and CommandService instance via ctor
        public CommandHandler(ILogger<CommandHandler> logger, DiscordSocketClient client, CommandService commands, Models.Config.Discord discordConfig, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _commands = commands;
            _client = client;
            _discordConfig = discordConfig;
            _serviceProvider = serviceProvider;

            _serviceScope = _serviceProvider.CreateScope();
            _scopedProvider = _serviceScope.ServiceProvider;

            //_commands.CommandExecuted += CommandExecutedAsync;
            //_client.MessageReceived += MessageReceivedAsync;
        }

        public async Task InstallCommandsAsync()
        {
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

        private async Task HandleCommandAsync(SocketMessage messageParam)
        {
            // Don't process the command if it was a system message
            var message = messageParam as SocketUserMessage;
            if (message == null) return;

            if (message.Author.IsBot) return;

            // Create a number to track where the prefix ends and the command begins
            int argPos = 0;

            // Determine if the message is a command based on the prefix and make sure no bots trigger commands
            if (
                    message.HasStringPrefix(_discordConfig.Prefix, ref argPos)
                    || message.HasMentionPrefix(_client.CurrentUser, ref argPos)
                    )
            {
                // Create a WebSocket-based command context based on the message
                var context = new SocketCommandContext(_client, message);

                // Execute the command with the command context we just
                // created, along with the service provider for precondition checks.
                /*
                using (var scope = _serviceProvider.CreateScope())
                {
                    await _commands.ExecuteAsync(
                        context: context,
                        argPos: argPos,
                        services: scope.ServiceProvider);
                }
                */
                await _commands.ExecuteAsync(
                        context: context,
                        argPos: argPos,
                        services: _scopedProvider);
            }
            else
            {
                var dmChannel = message.Channel as SocketDMChannel;
                if (dmChannel != null)
                {
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
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Unable to process DM");
                    }
                }
            }
        }
    }
}
