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
using DiscordBot.Domain.Entities.Alliances;
using DiscordBot.Domain.Entities.Request;
using DiscordBot.Domain.Shared;

namespace DiscordBot.Modules;

// [DefaultMemberPermissions(GuildPermission.ManageGuild)]
[Group("admin", "Admin Commands")]
public class AdminModule : BaseModule
{
    
    public AdminModule(ILogger<AdminModule> logger, IServiceProvider serviceProvider) : base(logger, serviceProvider)
    {
    }
    
    [SlashCommand("reload", "Bot Owner - Reload all data from database, without refreshing schedules.")]
    [RequireOwner]
    public async Task ReloadAsync()
    {
        using var serviceScope = ServiceProvider.CreateScope();
        _ = DeferAsync(false);
        try
        {
            await ModifyResponseAsync(
                "Reload initiated.",
                false);
            var cmdScheduler = ServiceProvider.GetService<Scheduler>();
            await cmdScheduler.ReloadJobsAsync(CancellationToken.None);
            await ModifyResponseAsync("Reload complete.", false);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, $"An unexpected error has occured while trying to run RELOAD.");
            await RespondAsync(
                "An unexpected error has occured.",
                ephemeral: false);
        }
    }
    
    [SlashCommand("echo", "Bot Owner = Echo text back to this channel")]
    public async Task EchoAsync(
        [Summary("Input", "Text to repeat")] string input)
    {
        if (Context.Channel is ISocketMessageChannel channel)
        {
            await channel.SendMessageAsync(input);
            await RespondAsync(
                "Done!",
                ephemeral: false);
        }
        else
        {
            await RespondAsync(
                "I can't do that; this doesn't look like a message channel.",
                ephemeral: false);
        }
    }
    
    [SlashCommand("get-role", "Get information about a specific role")]
    public async Task GetRoleId([Summary("Role")] IRole role)
    {
        if (Context.Channel is IPrivateChannel channel)
        {
            await RespondAsync("That command isn't valid in DMs.", ephemeral: false);
            return;
        }
        
        await RespondAsync($"Role {role.Name} ID = {role.Id}", ephemeral: false);
    }

    [SlashCommand("get-timestamp", "Converts a date/time to a timestamp to be used in Discord messages")]
    public async Task GetTimestamp(
        [Summary("Timezone", "Source timezone")] [Autocomplete(typeof(TimeZones))]
        string timezone,
        [Summary("Timestamp")] DateTime dateTime
    )
    {
        _ = DeferAsync(false);
        try
        {
            var sourceTimezone = TimeZoneInfo.FindSystemTimeZoneById(timezone);
            var destinationTimezone = TimeZoneInfo.Utc;
            var utcTime = TimeZoneInfo.ConvertTime(dateTime, sourceTimezone, destinationTimezone);
            await ModifyResponseAsync(
                $"Date/Time {dateTime} in {timezone} is {utcTime.ToUnixTimestamp()} seconds from Epoch; locally known as <t:{utcTime.ToUnixTimestamp()}>",
                false);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Unexpected exception getting timestamp for {Timezone} / {DateTime} on {Guild}", timezone, dateTime, Context.Guild.Id);
            await ModifyResponseAsync(
                "An unexpected error has occurred. If this continues, please contact the developer for support.",
                false);
        }
    }
    
    [SlashCommand("config", "Admin - Show or set a configuration variable for this Discord server")]
    public async Task ConfigAsync(
        [Summary("Name", "Name of variable to show or set")][Autocomplete(typeof(VariableNames))] string name,
        [Summary("Value","If provided, the new value for the variable. When applicable, set to None or -1 to clear")] string value = "")
    {
        if (Context.Channel is IPrivateChannel channel)
        {
            await RespondAsync("That command isn't valid in DMs.", ephemeral: false);
            return;
        }

        using var serviceScope = ServiceProvider.CreateScope();
        _ = DeferAsync(false);
        try
        {
            var allianceRepository = serviceScope.ServiceProvider.GetService<IAllianceRepository>();
            var thisAlliance = allianceRepository.FindFromGuildId(Context.Guild.Id);

            if (thisAlliance == null)
            {
                await ModifyResponseAsync("Unable to determine alliance from this channel", ephemeral: false);
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
                            false);
                        //await RespondAsync("No response was returned. Please check the current value or contact the developer.", ephemeral: true);
                    }
                    else
                    {
                        await ModifyResponseAsync(
                            responseBroadcastRole,
                            false);
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
                            false);
                        //await RespondAsync("No response was returned. Please check the current value or contact the developer.", ephemeral: true);
                    }
                    else
                    {
                        await ModifyResponseAsync(
                            responseBroadcastLeadTime,
                            false);
                        //await RespondAsync(response, ephemeral: true);
                    }

                    break;
                default:
                    await ModifyResponseAsync(
                        "The variable could not be identified.",
                        false);
                    //await RespondAsync("The variable could not be identified.", ephemeral: true);
                    break;
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Unexpected exception running config for {Name} = {Value} on {Guild}", name, value, Context.Guild.Id);
            await ModifyResponseAsync(
                "An unexpected error has occurred. If this continues, please contact the developer for support.",
                false);
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