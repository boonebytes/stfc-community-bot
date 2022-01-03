using System.Globalization;
using Discord;
using Discord.Commands;
using DiscordBot.Domain.Entities.Alliances;
using DiscordBot.Domain.Entities.Zones;
using DiscordBot.Domain.Shared;

namespace DiscordBot.Modules;

public partial class TdlModule
{
    [Command("zone set")]
    [Summary("Create or update a zone")]
    [RequireOwner]
    public async Task ZoneCreateUpdateAsync(string name, string owner, int level = 0, string threats = "",
        string dayOfWeekUtc = "", string timeOfDayUtc = "", [Remainder] string notes = "")
    {
        using var serviceScope = _serviceProvider.CreateScope();
        try
        {
            var zoneRepository = serviceScope.ServiceProvider.GetService<IZoneRepository>();
            var allianceRepository = serviceScope.ServiceProvider.GetService<IAllianceRepository>();

            if (level < 0 || level > 3)
            {
                await Context.Message.ReplyAsync(
                    "Level must be between 1 and 3. Specify 0 if you do not wish to change an existing value.");
                return;
            }

            Alliance ownerAlliance = null;
            if (owner != "" && owner != "0")
            {
                ownerAlliance = await allianceRepository.GetByNameOrAcronymAsync(owner);
                if (ownerAlliance == null)
                {
                    await Context.Message.ReplyAsync(
                        "The owner could not be found. Please check it and try again.");
                    return;
                }
            }

            if (!(new[] {"", "Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday"}.Contains(
                    dayOfWeekUtc)))
            {
                await Context.Message.ReplyAsync("The day of the week could not be detrmined.");
                return;
            }

            if (timeOfDayUtc != "")
            {
                DateTime verifyTimeOfDay;
                var culture = new CultureInfo("en-US");

                if (!DateTime.TryParseExact(timeOfDayUtc, "h:mm tt", culture, DateTimeStyles.AssumeUniversal,
                        out verifyTimeOfDay))
                {
                    await Context.Message.ReplyAsync("The time of day could not be understood.");
                    return;
                }
            }

            var zoneExists = await zoneRepository.GetByNameAsync(name);
            if (zoneExists == null)
            {
                // Create zone
                if (level == 0)
                {
                    await Context.Message.ReplyAsync("Level cannot be 0 for a new record.");
                    return;
                }

                if (dayOfWeekUtc == "")
                {
                    await Context.Message.ReplyAsync("The day of the week cannot be blank for a new record.");
                    return;
                }

                if (timeOfDayUtc == "")
                {
                    await Context.Message.ReplyAsync("The time of day cannot be blank for a new record.");
                    return;
                }

                Zone newZone = new(
                    0,
                    name,
                    level,
                    ownerAlliance,
                    threats,
                    dayOfWeekUtc,
                    timeOfDayUtc,
                    notes
                );
                zoneRepository.Add(newZone);
                await zoneRepository.UnitOfWork.SaveEntitiesAsync();
                await zoneRepository.InitZones();
                await Context.Message.ReplyAsync("Zone created");
            }
            else
            {
                // Update zone
                var newLevel = (level == 0 ? zoneExists.Level : level);
                var newDayOfWeek = (dayOfWeekUtc == "" || dayOfWeekUtc == "0"
                    ? zoneExists.DefendUtcDayOfWeek
                    : dayOfWeekUtc);
                var newTimeOfDay = (timeOfDayUtc == "" || timeOfDayUtc == "0"
                    ? zoneExists.DefendUtcTime
                    : timeOfDayUtc);

                string newThreats = zoneExists.Threats;
                if (threats == "null")
                {
                    newThreats = "";
                }
                else if (threats != "" && threats != "0")
                {
                    newThreats = threats;
                }

                string newNotes = zoneExists.Notes;
                if (notes == "null")
                {
                    newNotes = "";
                }
                else if (notes != "" && notes != "0")
                {
                    newNotes = notes;
                }

                zoneExists.Update(
                    zoneExists.Name,
                    level,
                    ownerAlliance,
                    newThreats,
                    newDayOfWeek,
                    newTimeOfDay,
                    newNotes
                );
                zoneRepository.Update(zoneExists);
                await zoneRepository.UnitOfWork.SaveEntitiesAsync();
                await zoneRepository.InitZones();
                await Context.Message.ReplyAsync("Zone updated");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                $"An unexpected error has occured while trying to run Zone Create Update for {Context.Guild.Name} in {Context.Channel.Name}.");
        }
    }

    [Command("connect")]
    [Summary("Register a connection between two zones")]
    [RequireOwner]
    public async Task ConnectAsync(string zone1, string zone2)
    {
        using var serviceScope = _serviceProvider.CreateScope();
        try
        {
            var zoneRepository = serviceScope.ServiceProvider.GetService<IZoneRepository>();
            var objZone1 = await zoneRepository.GetByNameAsync(zone1);
            var objZone2 = await zoneRepository.GetByNameAsync(zone2);

            if (objZone1 == null)
            {
                await Context.Message.ReplyAsync($"I'm sorry, I couldn't find a zone called {zone1}");
                return;
            }

            if (objZone2 == null)
            {
                await Context.Message.ReplyAsync($"I'm sorry, I couldn't find a zone called {zone2}");
                return;
            }

            string results = "";
            if (!objZone1.Neighbours.Contains(objZone2))
            {
                objZone1.AddNeighbour(objZone2);
                zoneRepository.Update(objZone1);
                results += $"Added {objZone1.Name} -> {objZone2.Name}\n";
            }

            if (!objZone2.Neighbours.Contains(objZone1))
            {
                objZone2.AddNeighbour(objZone1);
                zoneRepository.Update(objZone2);
                results += $"Added {objZone1.Name} <- {objZone2.Name}\n";
            }

            results = results.TrimEnd();

            if (results == "")
            {
                results = "No changes made.";
            }
            else
            {
                await zoneRepository.UnitOfWork.SaveEntitiesAsync();
            }

            await Context.Message.ReplyAsync(results);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                $"An unexpected error has occured while trying to run Connect for {Context.Guild.Name} in {Context.Channel.Name}.");
        }
    }

    [Command("zone show")]
    [Summary("Shows current zone info from the database")]
    public async Task ShowZoneAsync(string name)
    {
        using var serviceScope = _serviceProvider.CreateScope();
        try
        {
            var zoneRepository = serviceScope.ServiceProvider.GetService<IZoneRepository>();
            var allianceRepository = serviceScope.ServiceProvider.GetService<IAllianceRepository>();

            var thisZone = await zoneRepository.GetByNameAsync(name);
            if (thisZone == null)
            {
                await ReplyAsync("Zone not found.");
            }
            else
            {
                var potentialHostiles = zoneRepository
                    .GetPotentialHostiles(thisZone.Id)
                    .Select(a => a.Acronym)
                    .OrderBy(a => a)
                    .ToList();
                var potentialThreats = "";
                potentialThreats = potentialHostiles.Any() ? string.Join(", ", potentialHostiles) : "None";

                var owner = (thisZone.Owner == null ? "Unclaimed" : thisZone.Owner.Acronym);
                thisZone.SetNextDefend();
                string response =
                    $"Zone: {thisZone.Name} ({thisZone.Level}^)\n"
                    + $"Current Owner: {owner}\n"
                    + $"Next Event: <t:{thisZone.NextDefend.Value.ToUnixTimestamp()}> local / {thisZone.NextDefend.Value.ToEasternTime().ToString("h:mm tt")}\n"
                    + $"Potential Hostiles: {potentialThreats}\n";
                //if (!string.IsNullOrEmpty(thisZone.Threats))
                //    response += $"Saved Threats: {thisZone.Threats}\n";
                if (!string.IsNullOrEmpty(thisZone.Notes))
                    response += $"Notes: {thisZone.Notes}\n";
                await TryDeleteMessage(this.Context.Message);
                await ReplyAsync(response);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                $"An unexpected error has occured while trying to run Show Zone for {Context.Guild.Name} in {Context.Channel.Name}.");
        }
    }
}