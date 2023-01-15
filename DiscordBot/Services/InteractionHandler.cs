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

using System.Collections.Concurrent;
using System.Reflection;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using DiscordBot.Domain.Entities.Alliances;

namespace DiscordBot.Services;

public class InteractionHandler
{
    private readonly ILogger<InteractionHandler> _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly IServiceProvider _scopedProvider;
    private readonly DiscordSocketClient _client;
    private readonly InteractionService _interactionService;

    private static readonly ConcurrentDictionary<ulong, DateTime> _respondingInteractions = new();

    // Retrieve client and CommandService instance via ctor
    public InteractionHandler(ILogger<InteractionHandler> logger, DiscordSocketClient client, IServiceProvider serviceProvider)
    {
        _logger = logger;
        _client = client;
        _serviceProvider = serviceProvider;

        var serviceScope = _serviceProvider.CreateScope();
        _scopedProvider = serviceScope.ServiceProvider;
        
        _interactionService = new InteractionService(_client, new InteractionServiceConfig
        {
            WildCardExpression = "*",
            UseCompiledLambda = true,
            //DefaultRunMode = RunMode.Sync
        });
        
        // Hook the event into our command handler
        _client.InteractionCreated += InteractionCreatedAsync;
        _client.JoinedGuild += OnJoinedGuild;
    }
        
    public async Task InstallCommandsAsync()
    {
        _logger.LogInformation("Initializing the interaction handler");
        

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
            
        
#if DEBUG
        // For debugging, just register them to the sandbox server.
        await _interactionService.AddModulesAsync(Assembly.GetEntryAssembly(), _scopedProvider);
        await _interactionService.RegisterCommandsToGuildAsync(1024465149505060864);
#else

        // Single-use: Remove commands defined per-server
        /*
        var recognizedGuilds = _client.Guilds.Select(g => g.Id).ToArray();
        foreach (var guildId in recognizedGuilds)
        {
            await _interactionService.RegisterCommandsToGuildAsync(guildId, true);
        }
        */
        
        // Load the commands from the DLL, to be registered as global commans
        await _interactionService.AddModulesAsync(Assembly.GetEntryAssembly(), _scopedProvider);

        // Register the global commands.
        await _interactionService.RegisterCommandsGloballyAsync(true);
        _logger.LogInformation("Registered global interaction commands");
        
        var globalCommands = await _client.GetGlobalApplicationCommandsAsync();
        foreach (var cmd in globalCommands)
        {
            if (cmd.IsGlobalCommand)
            {
                _logger.LogInformation("Global Command {Id}: {Name} - {Description} (Created {Created})", cmd.Id, cmd.Name, cmd.Description, cmd.CreatedAt);
            }
            else
            {
                _logger.LogInformation("Command {Id}: {Name} - {Description} (Created {Created} / Guild {Guild})", cmd.Id, cmd.Name, cmd.Description, cmd.CreatedAt, cmd.Guild?.Id);
            }
        }

#endif
    }

    public static async Task CleanRespondingInteractionCache()
    {
        var removeItems = _respondingInteractions.Where(kvp => kvp.Value < DateTime.Now.AddHours(-2));
        foreach (var item in removeItems)
        {
            _respondingInteractions.TryRemove(item);
        }
    }

    private async Task OnJoinedGuild(SocketGuild guild)
    {
        _logger.LogInformation("Joined guild {GuildName} ({GuildId})", guild.Name, guild.Id);
    }

    private async Task RegisterInteractionsWithGuild(ulong guildId)
    {
        await _interactionService.RegisterCommandsToGuildAsync(guildId);
        _logger.LogInformation("Registered interaction commands to {GuildId}", guildId);
    }

    private async Task InteractionCreatedAsync(SocketInteraction arg)
    {
        var handling = _respondingInteractions.TryAdd(arg.Id, DateTime.Now);
        if (!handling) return;
        _ = RunInteractionCreatedAsync(arg);
    }
    
    protected async Task RunInteractionCreatedAsync(SocketInteraction arg)
    {
        _logger.LogDebug("Interaction started. ID = {ID} / Type = {Type} / Guild {Guild} ", arg.Id, arg.Type, arg.GuildId);
        if (arg is ISlashCommandInteraction slashCommandInteraction)
        {
            _logger.LogDebug("Slash command ID = {ID} / Name {Name}", 
                slashCommandInteraction.Data.Id,
                slashCommandInteraction.Data.Name);
        }
        var context = new SocketInteractionContext(_client, arg);
        var result = await _interactionService.ExecuteCommandAsync(context, _serviceProvider);

        if (!result.IsSuccess)
        {
            switch (result.Error)
            {
                case InteractionCommandError.UnmetPrecondition:
                    _logger.LogError("Unmet Precondition: {ErrorReason}", result.ErrorReason);
                    break;
                case InteractionCommandError.UnknownCommand:
                    _logger.LogError("Unknown command");
                    break;
                case InteractionCommandError.BadArgs:
                    _logger.LogError("Invalid number or arguments");
                    break;
                case InteractionCommandError.Exception:
                    _logger.LogError("Command exception:{ErrorReason}", result.ErrorReason);
                    break;
                case InteractionCommandError.Unsuccessful:
                    _logger.LogError("Command could not be executed");
                    break;
                default:
                    break;
            }
            //await interactionContext.Interaction.RespondAsync("An unexpected error has occured.", ephemeral: true);
        }

        //_logger.LogInformation("Interaction finished");
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
                    _logger.LogError("Unmet Precondition: {ErrorReason}", result.ErrorReason);
                    break;
                case InteractionCommandError.UnknownCommand:
                    _logger.LogError("Unknown command");
                    break;
                case InteractionCommandError.BadArgs:
                    _logger.LogError("Invalid number or arguments");
                    break;
                case InteractionCommandError.Exception:
                    _logger.LogError("Command exception:{ErrorReason}", result.ErrorReason);
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