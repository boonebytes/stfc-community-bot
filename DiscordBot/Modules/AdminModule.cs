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

[Discord.Interactions.Group("admin", "Admin Commands")]
public class AdminModule : BaseModule
{
    
    public AdminModule(ILogger<AdminModule> logger, IServiceProvider serviceProvider) : base(logger, serviceProvider)
    {
    }

    [RequireOwner]
    [SlashCommand("reload", "Bot Owner - Reload all data from database, without refreshing schedules.")]
    public async Task ReloadAsync()
    {
        using var serviceScope = _serviceProvider.CreateScope();
        _ = DeferAsync(true);
        try
        {
            await ModifyResponseAsync(
                "Reload initiated.",
                true);
            var cmdScheduler = _serviceProvider.GetService<Scheduler>();
            await cmdScheduler.ReloadJobsAsync(CancellationToken.None);
            await ModifyResponseAsync("Reload complete.", true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"An unexpected error has occured while trying to run RELOAD.");
            await RespondAsync(
                "An unexpected error has occured.",
                ephemeral: true);
        }
    }
        
    [RequireOwner]
    [SlashCommand("echo", "Bot Owner = Echo text back to this channel")]
    public async Task EchoAsync(
        [Summary("Input", "Text to repeat")] string input)
    {
        if (Context.Channel is ISocketMessageChannel channel)
        {
            await channel.SendMessageAsync(input);
            await RespondAsync(
                "Done!",
                ephemeral: true);
        }
        else
        {
            await RespondAsync(
                "I can't do that; this doesn't look like a message channel.",
                ephemeral: true);
        }
    }
    
    /*
    [SlashCommand("setup", "Admin - Check permissions, then configures the bot to post in this channel", runMode: RunMode.Async)]
    [RequireUserPermission(GuildPermission.Administrator)]
    public async Task SetupAsync()
    {
        await DeferAsync(ephemeral: true);
        
        _logger.LogInformation($"Setup started from {Context.Guild.Id} / {Context.Channel.Id}");
        using var serviceScope = _serviceProvider.CreateScope();
        var allianceRepository = serviceScope.ServiceProvider.GetService<IAllianceRepository>();
        var thisAlliance = allianceRepository.FindFromGuildId(Context.Guild.Id);

        if (Context.Channel is not SocketTextChannel channel)
        {
            _logger.LogInformation($"Unable to determine channel");
            await ModifyResponseAsync($"Unable to determine channel. Please ensure that a text channel is being used, then try again. Guild ${Context.Guild.Id}", ephemeral: true);
            return;
        }
        
        if (thisAlliance == null || thisAlliance.Id == 0)
        {
            _logger.LogInformation($"Server not initialized");
            await ModifyResponseAsync($"Server hasn't been initialized in the bot. Please provide these two IDs to the bot developer, along with your alliance name: Guild ${Context.Guild.Id} / Channel ${channel.Id}", ephemeral: true);
            return;
        }
        
        serviceScope.ServiceProvider.GetService<RequestContext>().Init(thisAlliance.Id);

        var guildUser = await Context.Guild.GetCurrentUserAsync();
        var channelPerms = guildUser.GetPermissions(channel);

        if (!(
                channelPerms.Has(ChannelPermission.ViewChannel)
                && channelPerms.Has(ChannelPermission.ReadMessageHistory)
                && channelPerms.Has(ChannelPermission.SendMessages)
                && channelPerms.Has(ChannelPermission.ManageMessages)
                && channelPerms.Has(ChannelPermission.MentionEveryone)))
        {
            _logger.LogInformation($"Channel missing permissions");
            await ModifyResponseAsync($"Bot doesn't have enough permissions in channel to post the schedule. Ensure these permissions have been granted: View Channel, Read Message History, Send Messages, Manage Messages, and Mention Everyone. Guild ${Context.Guild.Id} / Channel ${channel.Id}", ephemeral: true);
            return;
        }

        if (!thisAlliance.DefendSchedulePostChannel.HasValue)
        {
            thisAlliance.SetDefendSchedulePostChannel(channel.Id);
            allianceRepository.Update(thisAlliance);
            await allianceRepository.UnitOfWork.SaveEntitiesAsync();
            
            await ModifyResponseAsync(
                $"Added the {thisAlliance.Acronym} defense schedule for channel {channel.Id}. Everything looks good! Now posting the starting schedule. Please delete any bot posts above this line.",
                ephemeral: true);
        }
        else if (thisAlliance.DefendSchedulePostChannel.Value != channel.Id)
        {
            thisAlliance.SetDefendSchedulePostChannel(channel.Id);
            allianceRepository.Update(thisAlliance);
            await allianceRepository.UnitOfWork.SaveEntitiesAsync();
            
            await ModifyResponseAsync(
                $"Changed {thisAlliance.Acronym} defense schedule from Channel {thisAlliance.DefendSchedulePostChannel.Value} to {channel.Id}. Everything else looks good! Now posting the starting schedule. Please delete any bot posts above this line.",
                ephemeral: true);
        }
        else
        {
            await ModifyResponseAsync($"Bot seems to be good to go with these permissions. Guild ${Context.Guild.Id} / Channel ${channel.Id}", ephemeral: true);
        }
        
        var schedule = serviceScope.ServiceProvider.GetService<Responses.Schedule>();
        await schedule.PostAllAsync(Context.Guild.Id, channel.Id, thisAlliance.Id, true);
        _logger.LogInformation($"Setup complete");
    }
    */

    [SlashCommand("config", "Admin - Show or set a configuration variable for this Discord server")]
    [RequireUserPermission(GuildPermission.ManageGuild)]
    //[RequireOwner]
    public async Task ConfigAsync(
        [Summary("Name", "Name of variable to show or set")][Autocomplete(typeof(VariableNames))] string name,
        [Summary("Value","If provided, the new value for the variable. When applicable, set to None or -1 to clear")] string value = "")
    {
        using var serviceScope = _serviceProvider.CreateScope();
        _ = DeferAsync(true);
        try
        {
            var allianceRepository = serviceScope.ServiceProvider.GetService<IAllianceRepository>();
            var thisAlliance = allianceRepository.FindFromGuildId(Context.Guild.Id);

            if (thisAlliance == null)
            {
                await ModifyResponseAsync("Unable to determine alliance from this channel", ephemeral: true);
                return;
            }

            serviceScope.ServiceProvider.GetService<RequestContext>().Init(thisAlliance.Id);

            switch (name)
            {
                case VariableNames.VariableNameKeys.AlliedBroadcastRole:
                    var responseBroadcastRole =
                        await ConfigAlliedBroadcastRole(value, thisAlliance, allianceRepository);
                    if (responseBroadcastRole == "")
                    {
                        await ModifyResponseAsync(
                            "No response was returned. Please check the current value or contact the developer.",
                            true);
                        //await RespondAsync("No response was returned. Please check the current value or contact the developer.", ephemeral: true);
                    }
                    else
                    {
                        await ModifyResponseAsync(
                            responseBroadcastRole,
                            true);
                        //await RespondAsync(response, ephemeral: true);
                    }

                    break;
                case VariableNames.VariableNameKeys.BroadcastLeadTime:
                    var responseBroadcastLeadTime =
                        await ConfigDefendBroadcastTimeAsync(value, thisAlliance, allianceRepository);
                    if (responseBroadcastLeadTime == "")
                    {
                        await ModifyResponseAsync(
                            "No response was returned. Please check the current value or contact the developer.",
                            true);
                        //await RespondAsync("No response was returned. Please check the current value or contact the developer.", ephemeral: true);
                    }
                    else
                    {
                        await ModifyResponseAsync(
                            responseBroadcastLeadTime,
                            true);
                        //await RespondAsync(response, ephemeral: true);
                    }

                    break;
                default:
                    await ModifyResponseAsync(
                        "The variable could not be identified.",
                        true);
                    //await RespondAsync("The variable could not be identified.", ephemeral: true);
                    break;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected exception running config for {Name} = {Value} on {Guild}", name, value, Context.Guild.Id);
            await ModifyResponseAsync(
                "An unexpected error has occurred. If this continues, please contact the developer for support.",
                true);
        }
        
    }
    
    private async Task<string> ConfigAlliedBroadcastRole(string value, Alliance thisAlliance,
        IAllianceRepository allianceRepository)
    {
        if (value == "")
        {
            return thisAlliance.AlliedBroadcastRole.HasValue
                ? $"Current Value: {thisAlliance.AlliedBroadcastRole.Value}"
                : "No value has been set";
        }
        
        if (value == "-1" || value == "0" || value.ToLower() == "none")
        {
            thisAlliance.SetAlliedBroadcastRole(null);
            allianceRepository.Update(thisAlliance);
            await allianceRepository.UnitOfWork.SaveEntitiesAsync();
            return "Value cleared successfully";
        }

        if (!ulong.TryParse(value, out var newValue))
            return "The value provided could not be understood as a number.";
        
        thisAlliance.SetAlliedBroadcastRole(newValue);
        allianceRepository.Update(thisAlliance);
        await allianceRepository.UnitOfWork.SaveEntitiesAsync();
        return "Value updated successfully";
    }
    
    private async Task<string> ConfigDefendBroadcastTimeAsync(string value, Alliance thisAlliance,
        IAllianceRepository allianceRepository)
    {
        if (value == "")
        {
            return thisAlliance.DefendBroadcastLeadTime.HasValue
                ? $"Current Value: {thisAlliance.DefendBroadcastLeadTime.Value}"
                : "No value has been set";
        }
        
        if (value == "-1" || value.ToLower() == "none")
        {
            thisAlliance.SetDefendBroadcastLeadTime(null);
            allianceRepository.Update(thisAlliance);
            await allianceRepository.UnitOfWork.SaveEntitiesAsync();
            return "Value cleared successfully";
        }

        if (!int.TryParse(value, out int intValue))
            return "The value provided could not be understood as a number.";
        
        thisAlliance.SetDefendBroadcastLeadTime(intValue);
        allianceRepository.Update(thisAlliance);
        await allianceRepository.UnitOfWork.SaveEntitiesAsync();
        return "Value updated successfully";
    }
}