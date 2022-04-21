using Discord;
using Discord.Interactions;
using DiscordBot.Domain.Entities.Alliances;
using DiscordBot.Domain.Entities.Services;
using DiscordBot.Domain.Entities.Zones;
using DiscordBot.Domain.Shared;

namespace DiscordBot.Modules;

public partial class StfcModule
{
    [SlashCommand("alliance-show", "Shows information about an alliance")]
    public async Task AllianceShowAsync(
        [Summary("Name", "Name or acronym of the alliance to display")] string name)
    {
        using var serviceScope = _serviceProvider.CreateScope();
        try
        {
            var allianceRepository = serviceScope.ServiceProvider.GetService<IAllianceRepository>();

            var alliance = await allianceRepository.GetByNameOrAcronymAsync(name);
            if (alliance == null)
            {
                await this.RespondAsync(
                    "The alliance could not be found.",
                    ephemeral: true);
            }
            else
            {
                string response = $"Alliance: {alliance.Acronym} ({alliance.Name})\n"
                                  + $"Total Zones: {alliance.Zones.Count}\n";
                foreach (var zone in alliance.Zones.OrderBy(z => z.Level).ThenBy(z => z.Name))
                {
                    response += $"- {zone.Name} ({zone.Level}^)\n";
                }
                await RespondAsync(response);
            }
        }
        catch (Exception ex)
        {
            await RespondAsync(
                "An unexpected error has occured.",
                ephemeral: true);
            _logger.LogError(ex, "An unexpected error has occured while trying to run AllianceShowAsync");
        }
    }
    
    [SlashCommand("services-show", "Admin - Show alliance service costs")]
    [RequireUserPermission(GuildPermission.Administrator)]
    public async Task ServicesShowAsync()
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
        
        var serviceRepository = serviceScope.ServiceProvider.GetService<IServiceRepository>();
        var services = await serviceRepository.GetByAllianceIdAsync(thisAlliance.Id);

        var basicServices = services.Where(s =>
            s.AllianceServices.FirstOrDefault(allianceService => allianceService.Alliance == thisAlliance)?.AllianceServiceLevel == AllianceServiceLevel.Basic)
            .ToList();
        var enabledServices = services.Where(s =>
            new [] {AllianceServiceLevel.Basic, AllianceServiceLevel.Enabled}
                .Contains(s.AllianceServices.FirstOrDefault(allianceService => allianceService.Alliance == thisAlliance)?.AllianceServiceLevel))
            .ToList();
        var desiredServices = services.Where(s =>
            new [] {AllianceServiceLevel.Basic, AllianceServiceLevel.Enabled, AllianceServiceLevel.Desired}
                .Contains(s.AllianceServices.FirstOrDefault(allianceService => allianceService.Alliance == thisAlliance)?.AllianceServiceLevel))
            .ToList();

        var summary = "**__Summary_**\n\n";
        if (basicServices.Any())
        {
            summary += "**Basic Services:**\n";
            summary += getServiceCostSummary(basicServices) + "\n";
        }
        
        if (enabledServices.Any())
        {
            summary += "**Basic + Enabled Services:**\n";
            summary += getServiceCostSummary(enabledServices) + "\n";
        }
        
        if (desiredServices.Any())
        {
            summary += "**Basic + Enabled + Desired Services:**\n";
            summary += getServiceCostSummary(desiredServices) + "\n";
        }

        summary = summary.TrimEnd('\n');

        await Context.Channel.SendMessageAsync(summary);
    }

    private static string getServiceCostSummary(List<Service> services)
    {
        string result = "";
        var allCosts = services.SelectMany(s => s.Costs).ToList();
        foreach (var res in new[]
                 {
                     Resource.RefinedIsogenTier1, Resource.RefinedIsogenTier2, Resource.RefinedIsogenTier3,
                     Resource.ProgenitorDiodes, Resource.ProgenitorEmitters, Resource.ProgenitorReactors
                 })
        {
            var thisCost = allCosts.Where(c => c.Resource == res).Sum(c => c.Cost);
            result += "> " + res.Label + " = " + Functions.FriendlyNumberFormat(thisCost) + "\n";
        }

        return result;
    }

    /*
    [SlashCommand("alliance-set", "Bot Owner - Create or update an alliance")]
    [RequireOwner]
    public async Task AllianceCreateUpdateAsync(
        [Summary("Acronym", "Acronym of the alliance to add or update")] string acronym,
        [Summary("Name", "New name for the alliance")] string name,
        [Summary("Group", "Name of group to associate with alliance, or blank for ungrouped")] string group = "")
    {
        using var serviceScope = _serviceProvider.CreateScope();
        try
        {
            var allianceRepository = serviceScope.ServiceProvider.GetService<IAllianceRepository>();

            var allianceExists = await allianceRepository.GetByNameOrAcronymAsync(acronym);
            if (allianceExists == null)
            {
                allianceExists = await allianceRepository.GetByNameOrAcronymAsync(name);
            }


            AllianceGroup allianceGroup = null;
            if (@group != "" && @group != "0")
            {
                var allianceGroups = allianceRepository.GetAllianceGroups();
                allianceGroup = allianceGroups.FirstOrDefault(g => g.Name == @group);
            }

            if (allianceExists == null)
            {
                // Create alliance
                Alliance newAlliance = new(
                    0,
                    name,
                    acronym,
                    allianceGroup,
                    null,
                    null,
                    null,
                    null);
                allianceRepository.Add(newAlliance);
                await allianceRepository.UnitOfWork.SaveEntitiesAsync();
                await RespondAsync(
                    "Alliance created",
                    ephemeral: true);
            }
            else
            {
                // Edit alliance
                allianceExists.Update(
                    name,
                    acronym,
                    allianceGroup,
                    allianceExists.GuildId,
                    allianceExists.DefendSchedulePostChannel,
                    allianceExists.DefendSchedulePostTime,
                    allianceExists.DefendBroadcastLeadTime);
                allianceRepository.Update(allianceExists);
                await allianceRepository.UnitOfWork.SaveEntitiesAsync();
                await RespondAsync(
                    "Alliance updated",
                    ephemeral: true);
            }
        }
        catch (Exception ex)
        {
            await RespondAsync(
                "An unexpected error has occured.",
                ephemeral: true);
            _logger.LogError(ex, "An unexpected error has occured while trying to run AllianceCreateUpdateAsync");
        }
    }

    [SlashCommand("alliance-rename","Bot Owner - Rename an alliance")]
    [RequireOwner]
    public async Task AllianceRenameAsync(
        [Summary("Old-Name","Current name or acronym")] string oldNameOrAcronym,
        [Summary("New-Acronym", "New Acronym")] string newAcronym,
        [Summary("New-Name","New name for alliance")] string newName = "",
        [Summary("New-Group", "New group for alliance, -1 to clear current group, blank to leave unchanged")] string newGroup = "")
    {
        using var serviceScope = _serviceProvider.CreateScope();
        try
        {
            var allianceRepository = serviceScope.ServiceProvider.GetService<IAllianceRepository>();

            var allianceExists = await allianceRepository.GetByNameOrAcronymAsync(oldNameOrAcronym);
            if (allianceExists == null)
            {
                await RespondAsync(
                    "Unable to find old alliance by provided acronym",
                    ephemeral: true);
            }
            else
            {
                AllianceGroup allianceGroup = allianceExists.Group;

                if (newGroup == "-1")
                {
                    allianceGroup = null;
                }
                else if (newGroup != "" && newGroup != "0")
                {
                    var allianceGroups = allianceRepository.GetAllianceGroups();
                    allianceGroup = allianceGroups.FirstOrDefault(g => g.Name == newGroup);
                }

                // Edit alliance
                allianceExists.Update(
                    newName,
                    newAcronym,
                    allianceGroup,
                    allianceExists.GuildId,
                    allianceExists.DefendSchedulePostChannel,
                    allianceExists.DefendSchedulePostTime,
                    allianceExists.DefendBroadcastLeadTime);
                allianceRepository.Update(allianceExists);
                await allianceRepository.UnitOfWork.SaveEntitiesAsync();
                await RespondAsync(
                    "Alliance updated",
                    ephemeral: true);
            }
        }
        catch (Exception ex)
        {
            await RespondAsync(
                "An unexpected error has occured.",
                ephemeral: true);
            _logger.LogError(ex,
                $"An unexpected error has occured while trying to rename the {oldNameOrAcronym} alliance.");
        }
    }
    */
}