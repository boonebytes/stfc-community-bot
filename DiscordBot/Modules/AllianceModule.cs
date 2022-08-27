using Discord;
using Discord.Interactions;
using DiscordBot.Domain.Entities.Alliances;
using DiscordBot.Domain.Entities.Request;
using DiscordBot.Domain.Entities.Services;
using DiscordBot.Domain.Entities.Zones;
using DiscordBot.Domain.Shared;

namespace DiscordBot.Modules;

[Discord.Interactions.Group("alliance", "Show Alliance Info")]
public class AllianceModule : InteractionModuleBase<SocketInteractionContext>
{
    private readonly ILogger<AllianceModule> _logger;
    private readonly IServiceProvider _serviceProvider;

    public AllianceModule(ILogger<AllianceModule> logger, IServiceProvider serviceProvider)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
    }
    
    private async Task ModifyResponseAsync(string content = "", bool ephemeral = false, Embed embed = null)
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
    
    [SlashCommand("show", "Shows information about an alliance")]
    public async Task AllianceShowAsync(
        [Summary("Name", "Name or acronym of the alliance to display")] string name)
    {
        using var serviceScope = _serviceProvider.CreateScope();
        try
        {
            var allianceRepository = serviceScope.ServiceProvider.GetService<IAllianceRepository>();

            var thisAlliance = allianceRepository.FindFromGuildId(Context.Guild.Id);
            serviceScope.ServiceProvider.GetService<RequestContext>().Init(thisAlliance.Id);
            
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
    
    [SlashCommand("services", "Show alliance service costs")]
    [RequireUserPermission(GuildPermission.ManageGuild)]
    public async Task ServicesShowAsync()
    {
        try
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
            
            serviceScope.ServiceProvider.GetService<RequestContext>().Init(thisAlliance.Id);
            
            var serviceRepository = serviceScope.ServiceProvider.GetService<IServiceRepository>();

            var countedServices = new List<long>();

            var basicServices =
                await serviceRepository.GetCostByAllianceServiceLevelAsync(thisAlliance.Id, AllianceServiceLevel.Basic);
            var preferredServicesRaw =
                await serviceRepository.GetCostByAllianceServiceLevelAsync(thisAlliance.Id,
                    AllianceServiceLevel.Preferred);
            var desiredServicesRaw =
                await serviceRepository.GetCostByAllianceServiceLevelAsync(thisAlliance.Id,
                    AllianceServiceLevel.Desired);

            var preferredServices = preferredServicesRaw;
            foreach (var service in basicServices)
            {
                if (preferredServices.ContainsKey(service.Key))
                {
                    preferredServices[service.Key] += service.Value;
                }
                else
                {
                    preferredServices.Add(service.Key, service.Value);
                }
            }

            var desiredServices = desiredServicesRaw;
            foreach (var service in preferredServices)
            {
                if (desiredServices.ContainsKey(service.Key))
                {
                    desiredServices[service.Key] += service.Value;
                }
                else
                {
                    desiredServices.Add(service.Key, service.Value);
                }
            }
            
            const string serviceHeader = "**__Service Cost Summary__**";
            var summary = serviceHeader + "\n\n";
            if (basicServices.Any())
            {
                summary += "**Basic Services:**\n";
                summary += getServiceCostSummary(basicServices) + "\n";
            }

            if (preferredServices.Any())
            {
                summary += "**Basic + Preferred Services:**\n";
                summary += getServiceCostSummary(preferredServices) + "\n";
            }

            if (desiredServices.Any())
            {
                summary += "**Basic + Preferred + Desired Services:**\n";
                summary += getServiceCostSummary(desiredServices) + "\n";
            }

            summary = summary.TrimEnd('\n');


            var channelMessages = await Context.Channel.GetMessagesAsync().FlattenAsync();
            var botServiceSummaryMessage = (Discord.Rest.RestUserMessage)channelMessages.FirstOrDefault(m => 
                m.Author.Id == Context.Client.CurrentUser.Id
                && m.Content.StartsWith(serviceHeader));

            if (botServiceSummaryMessage == null)
            {
                await Context.Channel.SendMessageAsync(summary);
            }
            else
            {
                await botServiceSummaryMessage.ModifyAsync(m => m.Content = summary);
            }
            
            await ModifyResponseAsync("Done!", true);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error in AllianceServiceCostSummary");
        }
    }

    private static string getServiceCostSummary(Dictionary<Resource, long> allCosts)
    {
        string result = "";
        foreach (var res in new[]
                 {
                     Resource.RefinedIsogenTier1, Resource.RefinedIsogenTier2, Resource.RefinedIsogenTier3,
                     Resource.ProgenitorDiodes, Resource.ProgenitorEmitters, Resource.ProgenitorReactors
                 })
        {
            if (allCosts.ContainsKey(res) && allCosts[res] > 0)
            {
                result += "> " + res.Label + " = " + Functions.FriendlyNumberFormat(allCosts[res]) + "\n";
            }
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