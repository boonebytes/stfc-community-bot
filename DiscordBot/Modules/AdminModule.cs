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
using Discord.Interactions;
using Discord.WebSocket;
using DiscordBot.AutocompleteHandlers;
using DiscordBot.Domain.Entities.Admin;
using DiscordBot.Domain.Entities.Alliances;
using DiscordBot.Domain.Entities.Request;
using DiscordBot.Domain.Shared;

namespace DiscordBot.Modules;

[DefaultMemberPermissions(GuildPermission.ManageGuild)]
[Group("admin", "Admin Commands")]
public class AdminModule : BaseModule
{
    
    public AdminModule(ILogger<AdminModule> logger, IServiceProvider serviceProvider) : base(logger, serviceProvider)
    {
    }

    [SlashCommand("schedule-message", "Admin - Schedule a message to be posted at a specific date/time")]
    [RequireUserPermission(GuildPermission.ManageGuild, Group = "Permission")]
    [RequireOwner(Group = "Permission")]
    public async Task ScheduledMessageAsync(
        [Summary("Timezone", "Source timezone")] [Autocomplete(typeof(TimeZones))]  string timezone,
        [Summary("Timestamp", "Date/Time to post the message")] DateTime dateTime,
        [Summary("Message", "Message to post")] string message,
        [Summary("Channel", "Channel to post; this one if not specified")] ITextChannel channel = null
        )
    {
        using var serviceScope = ServiceProvider.CreateScope();
        _ = DeferAsync(true);
        
        if (Context.Guild == null)
        {
            await ModifyResponseAsync(
                "I can't do that; I can't detect the alliance from this channel.",
                ephemeral: true);
            return;
        }

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

            var scheduledChannel = channel;
            if (scheduledChannel == null)
            {
                if (Context.Channel is not ITextChannel currentChannel)
                {
                    await ModifyResponseAsync(
                        "I cannot find the channel specified.",
                        ephemeral: true);
                    return;
                }

                scheduledChannel = currentChannel;
            }

            if (scheduledChannel.Guild != Context.Guild)
            {
                await ModifyResponseAsync(
                    "I cannot find the channel specified.",
                    ephemeral: true);
                return;
            }

            DateTime utcTime = DateTime.MaxValue;
            try
            {
                var sourceTimezone = TimeZoneInfo.FindSystemTimeZoneById(timezone);
                var destinationTimezone = TimeZoneInfo.Utc;
                utcTime = TimeZoneInfo.ConvertTime(dateTime, sourceTimezone, destinationTimezone);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Unexpected exception getting timestamp for {Timezone} / {DateTime} on {Guild}",
                    timezone, dateTime, Context.Guild.Id);
                await ModifyResponseAsync(
                    "An unexpected error has occurred. If this continues, please contact the developer for support.",
                    true);
                return;
            }

            if (utcTime == DateTime.MaxValue)
            {
                Logger.LogError("DateTime did not return a proper value {Timezone} / {DateTime} on {Guild}", timezone,
                    dateTime, Context.Guild.Id);
                await ModifyResponseAsync(
                    "An unexpected error has occurred. If this continues, please contact the developer for support.",
                    true);
                return;
            }

            var guildUser = Context.Guild.GetUser(Context.User.Id);
            var nickname = guildUser.Nickname;

            var newJob = new CustomMessageJob(
                utcTime,
                Context.User.Id,
                Context.User.Username + '#' + Context.User.Discriminator,
                nickname,
                thisAlliance,
                scheduledChannel.Id,
                message);
            newJob.Schedule();

            var customMessageJobRepository = serviceScope.ServiceProvider.GetService<ICustomMessageJobRepository>();
            customMessageJobRepository.Add(newJob);
            await customMessageJobRepository.UnitOfWork.SaveEntitiesAsync();
            
            await ModifyResponseAsync(
                $"Done! Scheduled for <t:{utcTime.ToUnixTimestamp()}>",
                true);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Unexpected exception creating custom scheduled message for {UserId}",
                 Context.User.Id);
            await ModifyResponseAsync(
                "An unexpected error has occurred. If this continues, please contact the developer for support.",
                true);
        }
    }

    [SlashCommand("reload", "Bot Owner - Reload all data from database, without refreshing schedules.")]
    [RequireOwner]
    public async Task ReloadAsync()
    {
        using var serviceScope = ServiceProvider.CreateScope();
        _ = DeferAsync(true);
        try
        {
            await ModifyResponseAsync(
                "Reload initiated.",
                true);
            var cmdScheduler = ServiceProvider.GetService<Scheduler>();
            await cmdScheduler.ReloadJobsAsync(CancellationToken.None);
            await ModifyResponseAsync("Reload complete.", true);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, $"An unexpected error has occured while trying to run RELOAD.");
            await RespondAsync(
                "An unexpected error has occured.",
                ephemeral: true);
        }
    }
    
    [SlashCommand("echo", "Bot Owner = Echo text back to this channel")]
    [RequireOwner]
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
    
    [SlashCommand("get-role", "Get information about a specific role")]
    [RequireUserPermission(GuildPermission.ManageGuild, Group = "Permission")]
    [RequireOwner(Group = "Permission")]
    public async Task GetRoleId([Summary("Role")] IRole role)
    {
        if (Context.Channel is IPrivateChannel channel)
        {
            await RespondAsync("That command isn't valid in DMs.", ephemeral: true);
            return;
        }
        
        await RespondAsync($"Role {role.Name} ID = {role.Id}", ephemeral: true);
    }

    [SlashCommand("get-timestamp", "Converts a date/time to a timestamp to be used in Discord messages")]
    [RequireUserPermission(GuildPermission.SendMessages)]
    public async Task GetTimestamp(
        [Summary("Timezone", "Source timezone")] [Autocomplete(typeof(TimeZones))]
        string timezone,
        [Summary("Timestamp")] DateTime dateTime
    )
    {
        _ = DeferAsync(true);
        try
        {
            var sourceTimezone = TimeZoneInfo.FindSystemTimeZoneById(timezone);
            var destinationTimezone = TimeZoneInfo.Utc;
            var utcTime = TimeZoneInfo.ConvertTime(dateTime, sourceTimezone, destinationTimezone);
            await ModifyResponseAsync(
                $"Date/Time {dateTime} in {timezone} is {utcTime.ToUnixTimestamp()} seconds from Epoch; locally known as <t:{utcTime.ToUnixTimestamp()}>",
                true);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Unexpected exception getting timestamp for {Timezone} / {DateTime} on {Guild}", timezone, dateTime, Context.Guild.Id);
            await ModifyResponseAsync(
                "An unexpected error has occurred. If this continues, please contact the developer for support.",
                true);
        }
    }
    
    [SlashCommand("config", "Admin - Show or set a configuration variable for this Discord server")]
    [RequireUserPermission(GuildPermission.ManageGuild, Group = "Permission")]
    [RequireOwner(Group = "Permission")]
    public async Task ConfigAsync(
        [Summary("Name", "Name of variable to show or set")][Autocomplete(typeof(VariableNames))] string name,
        [Summary("Value","If provided, the new value for the variable. When applicable, set to None or -1 to clear")] string value = "")
    {
        if (Context.Channel is IPrivateChannel channel)
        {
            await RespondAsync("That command isn't valid in DMs.", ephemeral: true);
            return;
        }

        using var serviceScope = ServiceProvider.CreateScope();
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
            Logger.LogError(ex, "Unexpected exception running config for {Name} = {Value} on {Guild}", name, value, Context.Guild.Id);
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