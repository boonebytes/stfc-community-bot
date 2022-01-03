using System.Reflection;
using Discord.Interactions;
using Discord.WebSocket;
using DiscordBot.Domain.Entities.Alliances;

namespace DiscordBot.Services;

public class InteractionHandler
{
    private readonly ILogger<InteractionHandler> _logger;
    private readonly Models.Config.Discord _discordConfig;
    private readonly IServiceProvider _serviceProvider;
    private readonly IServiceProvider _scopedProvider;
    private readonly DiscordSocketClient _client;
    private readonly InteractionService _interactionService;

    // Retrieve client and CommandService instance via ctor
    public InteractionHandler(ILogger<InteractionHandler> logger, DiscordSocketClient client, InteractionService interactionService, Models.Config.Discord discordConfig, IServiceProvider serviceProvider)
    {
        _logger = logger;
        _interactionService = interactionService;
        _client = client;
        _discordConfig = discordConfig;
        _serviceProvider = serviceProvider;

        var serviceScope = _serviceProvider.CreateScope();
        _scopedProvider = serviceScope.ServiceProvider;
    }
        
    public async Task InstallCommandsAsync()
    {
        // Hook the event into our command handler
        //_client.MessageReceived += HandleCommandAsync;
        ////_client.SlashCommandExecuted += SlashCommandHandlerAsync;
        //_client.SlashCommandExecuted += SlashCommandHandlerAsync;
        _client.InteractionCreated += InteractionCreatedAsync;
        //_client.SlashCommandExecuted += HandleSlashCommandAsync;
        _interactionService.SlashCommandExecuted += SlashCommandExecutedAsync;


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
            
        var modules = await _interactionService.AddModulesAsync(Assembly.GetEntryAssembly(), _scopedProvider);
//#if DEBUG
//            await _interactionService.RegisterCommandsToGuildAsync(671097115233091630);
//#else
        var allianceRepository = _serviceProvider.GetService<IAllianceRepository>();
        var allDiscordServers = allianceRepository.GetAllWithServers();
        foreach (var alliance in allDiscordServers)
        {
            if (alliance.GuildId.HasValue)
                await _interactionService.RegisterCommandsToGuildAsync(alliance.GuildId.Value);
        }
        //await _interactionService.RegisterCommandsGloballyAsync();
//#endif
    }

    private async Task InteractionCreatedAsync(SocketInteraction arg)
    {
        var context = new SocketInteractionContext(_client, arg);
        await _interactionService.ExecuteCommandAsync(context, _serviceProvider);
    }

    //private async Task SlashCommandHandlerAsync(SocketSlashCommand command)
    //{
    //    var commandContext = new SocketInteractionContext(_client, command);
    //    await _interactionService.ExecuteCommandAsync(commandContext, _serviceProvider);
    //}
        
    private async Task SlashCommandExecutedAsync(SlashCommandInfo slashCommandInfo,
        Discord.IInteractionContext interactionContext, Discord.Interactions.IResult result)
    {
        if (!result.IsSuccess)
        {
            switch (result.Error)
            {
                case InteractionCommandError.UnmetPrecondition:
                    _logger.LogError($"Unmet Precondition: {result.ErrorReason}");
                    break;
                case InteractionCommandError.UnknownCommand:
                    _logger.LogError("Unknown command");
                    break;
                case InteractionCommandError.BadArgs:
                    _logger.LogError("Invalid number or arguments");
                    break;
                case InteractionCommandError.Exception:
                    _logger.LogError($"Command exception:{result.ErrorReason}");
                    break;
                case InteractionCommandError.Unsuccessful:
                    _logger.LogError("Command could not be executed");
                    break;
                default:
                    break;
            }

            await interactionContext.Interaction.RespondAsync("An unexpected error has occured.", ephemeral: true);
        }
    }
    /*
    private async Task HandleSlashCommandAsync(SocketSlashCommand command)
    {
        switch (command.Data.Name)
        {
            case "command-1":
                
        }
    }
    */
}