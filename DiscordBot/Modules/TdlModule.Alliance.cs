using System;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using DiscordBot.Domain.Entities.Alliances;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace DiscordBot.Modules
{
    public partial class TdlModule
    {
        [Command("alliance show")]
        [Summary("Shows information about an alliance")]
        public async Task AllianceShowAsync(string name)
        {
            using var serviceScope = _serviceProvider.CreateScope();
            try
            {
                var allianceRepository = serviceScope.ServiceProvider.GetService<IAllianceRepository>();

                var alliance = await allianceRepository.GetByNameOrAcronymAsync(name);
                if (alliance == null)
                {
                    await this.ReplyAsync("The alliance could not be found.");
                }
                else
                {
                    string response = $"Alliance: {alliance.Acronym} ({alliance.Name})\n"
                                      + $"Total Zones: {alliance.Zones.Count}\n";
                    foreach (var zone in alliance.Zones.OrderBy(z => z.Level).ThenBy(z => z.Name))
                    {
                        response += $"- {zone.Name} ({zone.Level}^)\n";
                    }
                    await TryDeleteMessage(this.Context.Message);
                    await this.ReplyAsync(response);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error has occured while trying to run AllianceShowAsync");
            }
        }
        
        [Command("alliance set")]
        [Summary("Create or update an alliance")]
        [RequireOwner]
        public async Task AllianceCreateUpdateAsync(string acronym, string name, string group = "")
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
                    await Context.Message.ReplyAsync("Alliance created");
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
                    await Context.Message.ReplyAsync("Alliance updated");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error has occured while trying to run AllianceCreateUpdateAsync");
            }
        }

        [Command("alliance rename")]
        [Summary("Rename an alliance")]
        [RequireOwner]
        public async Task AllianceRenameAsync(string oldNameOrAcronym, string newAcronym, string newName = "",
            string newGroup = "")
        {
            using var serviceScope = _serviceProvider.CreateScope();
            try
            {
                var allianceRepository = serviceScope.ServiceProvider.GetService<IAllianceRepository>();

                var allianceExists = await allianceRepository.GetByNameOrAcronymAsync(oldNameOrAcronym);
                if (allianceExists == null)
                {
                    await this.ReplyAsync(("Unable to find old alliance by provided acronym"));
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
                    await Context.Message.ReplyAsync("Alliance updated");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    $"An unexpected error has occured while trying to rename the {oldNameOrAcronym} alliance.");
            }
        }
    }
}