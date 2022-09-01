using System.Collections.Immutable;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using DiscordBot.AutocompleteHandlers;
using DiscordBot.Domain.Entities.Alliances;
using DiscordBot.Domain.Entities.Request;
using DiscordBot.Domain.Entities.Zones;
using DiscordBot.Domain.Exceptions;

namespace DiscordBot.Modules;

public abstract class BaseModule : InteractionModuleBase<SocketInteractionContext>
{
    protected readonly ILogger<BaseModule> _logger;
    protected readonly IServiceProvider _serviceProvider;

    public BaseModule(ILogger<BaseModule> logger, IServiceProvider serviceProvider)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
    }
    
    protected async Task ModifyResponseAsync(string content = "", bool ephemeral = false, Embed embed = null)
    {
        await Context.Interaction.ModifyOriginalResponseAsync(properties =>
        {
            if (embed != null) properties.Embed = embed;
            properties.Content = content;
            if (ephemeral)
                properties.Flags = MessageFlags.Ephemeral;
            else
                properties.Flags = MessageFlags.None;
        });
    }
}