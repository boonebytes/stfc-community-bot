using Discord;
using Discord.Interactions;
using DiscordBot.Domain.Entities.Alliances;
using DiscordBot.Domain.Entities.Request;
using DiscordBot.Domain.Entities.Services;
using DiscordBot.Domain.Entities.Zones;
using DiscordBot.Domain.Shared;

namespace DiscordBot.Modules;

// [DefaultMemberPermissions(GuildPermission.ManageGuild)]
[Group("alliance", "Show Alliance Info")]
public class AllianceModule : BaseModule
{

    public AllianceModule(ILogger<AllianceModule> logger, IServiceProvider serviceProvider) : base(logger, serviceProvider)
    {
    }
    
    [SlashCommand("show", "Shows information about an alliance")]
    [RequireUserPermission(ChannelPermission.SendMessages)]
    public async Task AllianceShowAsync(
        [Summary("Name", "Name or acronym of the alliance to display")] string name)
    {
        using var serviceScope = ServiceProvider.CreateScope();
        try
        {
            var allianceRepository = serviceScope.ServiceProvider.GetService<IAllianceRepository>();

            if (Context.Channel is IPrivateChannel)
            {
                serviceScope.ServiceProvider.GetService<RequestContext>().Init(null);
            }
            else
            {
                var thisAlliance = allianceRepository.FindFromGuildId(Context.Guild.Id);
                serviceScope.ServiceProvider.GetService<RequestContext>().Init(thisAlliance.Id);
            }
            
            
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
                foreach (var zone in alliance.Zones.OrderBy(z => z.NextDefend).ThenBy(z => z.Name))
                {
                    response += $"- {zone.Name} ({zone.Level}^) - {zone.NextDefend?.ToEasternTime().ToString("MMM d h:mm tt")} ET\n";
                }
                await RespondAsync(response);
            }
        }
        catch (Exception ex)
        {
            await RespondAsync(
                "An unexpected error has occured.",
                ephemeral: true);
            Logger.LogError(ex, "An unexpected error has occured while trying to run AllianceShowAsync");
        }
    }
    
    [SlashCommand("services", "Show alliance service costs")]
    [RequireUserPermission(GuildPermission.ManageGuild, Group = "Permission")]
    [RequireOwner(Group = "Permission")]
    public async Task ServicesShowAsync()
    {
        if (Context.Channel is IPrivateChannel channel)
        {
            await RespondAsync("That command isn't valid in DMs.", ephemeral: true);
            return;
        }
        
        try
        {
            using var serviceScope = ServiceProvider.CreateScope();
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
                summary += GetServiceCostSummary(basicServices) + "\n";
            }

            if (preferredServices.Any())
            {
                summary += "**Basic + Preferred Services:**\n";
                summary += GetServiceCostSummary(preferredServices) + "\n";
            }

            if (desiredServices.Any())
            {
                summary += "**Basic + Preferred + Desired Services:**\n";
                summary += GetServiceCostSummary(desiredServices) + "\n";
            }

            summary = summary.TrimEnd('\n');

            await Context.Channel.SendMessageAsync(summary);
            /*
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
            */
            
            await ModifyResponseAsync("Done!", true);
        }
        catch (Exception e)
        {
            Logger.LogError(e, "Error in AllianceServiceCostSummary");
        }
    }

    private static string GetServiceCostSummary(Dictionary<Resource, long> allCosts)
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
}