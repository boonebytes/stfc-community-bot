using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using DiscordBot.Domain.Entities.Alliances;
using DiscordBot.Domain.Entities.Zones;
using DiscordBot.Domain.Exceptions;
using DiscordBot.Domain.Shared;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace DiscordBot.Modules
{
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
        
        [SlashCommand("alliance-set", "Create or update an alliance")]
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
                        allianceExists.DefendSchedulePostTime);
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

        [SlashCommand("alliance-rename","Rename an alliance")]
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
                        allianceExists.DefendSchedulePostTime);
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
    }
}