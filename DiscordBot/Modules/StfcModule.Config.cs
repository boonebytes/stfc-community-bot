using Discord;
using Discord.Interactions;
using DiscordBot.AutocompleteHandlers;
using DiscordBot.Domain.Entities.Alliances;

namespace DiscordBot.Modules;

public partial class StfcModule
{
    [SlashCommand("config", "Show or set a configuration variable for this Discord server")]
    //[RequireUserPermission(GuildPermission.Administrator)]
    [RequireOwner]
    public async Task ConfigAsync(
        [Summary("Name", "Name of variable to show or set")][Autocomplete(typeof(VariableNames))] string name,
        [Summary("Value","If provided, the new value for the variable. When applicable, set to None or -1 to clear")] string value = "")
    {
        using var serviceScope = _serviceProvider.CreateScope();
        var allianceRepository = serviceScope.ServiceProvider.GetService<IAllianceRepository>();
        var thisAlliance = allianceRepository.FindFromGuildId(Context.Guild.Id);

        if (thisAlliance == null)
        {
            await RespondAsync("Unable to determine alliance from this channel", ephemeral: true);
            return;
        }

        await DeferAsync(ephemeral: true);
        switch (name)
        {
            case VariableNames.VariableNameKeys.BroadcastLeadTime:
                var response = await ConfigDefendBroadcastTimeAsync(value, thisAlliance, allianceRepository);
                if (response == "")
                {
                    await ModifyResponseAsync(
                        "No response was returned. Please check the current value or contact the developer.",
                        true);
                    //await RespondAsync("No response was returned. Please check the current value or contact the developer.", ephemeral: true);
                }
                else
                {
                    await ModifyResponseAsync(
                        response,
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