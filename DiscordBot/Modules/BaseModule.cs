using Discord;
using Discord.Interactions;

namespace DiscordBot.Modules;

public abstract class BaseModule : InteractionModuleBase<SocketInteractionContext>
{
    protected readonly ILogger<BaseModule> Logger;
    protected readonly IServiceProvider ServiceProvider;

    protected BaseModule(ILogger<BaseModule> logger, IServiceProvider serviceProvider)
    {
        Logger = logger;
        ServiceProvider = serviceProvider;
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