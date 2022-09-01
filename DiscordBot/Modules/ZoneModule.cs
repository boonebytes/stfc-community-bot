using System.Globalization;
using Discord;
using Discord.Interactions;
using DiscordBot.AutocompleteHandlers;
using DiscordBot.Domain.Entities.Alliances;
using DiscordBot.Domain.Entities.Request;
using DiscordBot.Domain.Entities.Zones;
using DiscordBot.Domain.Shared;

namespace DiscordBot.Modules;

[Discord.Interactions.Group("zone", "Show / Edit Zone Info")]
public class ZoneModule : BaseModule
{
    public ZoneModule(ILogger<ZoneModule> logger, IServiceProvider serviceProvider) : base(logger, serviceProvider)
    {
    }
    
    [SlashCommand("set", "Bot Owner - Create or update a zone")]
    [RequireOwner]
    public async Task ZoneCreateUpdateAsync(
        [Summary("Zone", "Zone Name")][Autocomplete(typeof(ZoneNames))] string name,
        [Summary("Owner", "Current owner, or None if unclaimed")] string owner,
        [Summary("Level", "Zone level, or 0 if unchanged")][MinValue(0)][MaxValue(3)] int level = 0,
        string dayOfWeekUtc = "",
        string timeOfDayUtc = "",
        string notes = "")
    {
        using var serviceScope = _serviceProvider.CreateScope();
        await this.DeferAsync(ephemeral: true);
        try
        {
            var zoneRepository = serviceScope.ServiceProvider.GetService<IZoneRepository>();
            var allianceRepository = serviceScope.ServiceProvider.GetService<IAllianceRepository>();

            if (level < 0 || level > 3)
            {
                await ModifyResponseAsync(
                    "Level must be between 1 and 3. Specify 0 if you do not wish to change an existing value.",
                    ephemeral: true);
                return;
            }

            Alliance ownerAlliance = null;
            if (owner != "" && owner != "0" && owner.ToLower() != "none")
            {
                ownerAlliance = await allianceRepository.GetByNameOrAcronymAsync(owner);
                if (ownerAlliance == null)
                {
                    await ModifyResponseAsync(
                        "The owner could not be found. Please check it and try again.",
                        ephemeral: true);
                    return;
                }
            }

            if (!(new[] {"", "Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday"}.Contains(
                    dayOfWeekUtc)))
            {
                await ModifyResponseAsync(
                    "The day of the week could not be determined.",
                    ephemeral: true);
                return;
            }

            if (timeOfDayUtc != "")
            {
                DateTime verifyTimeOfDay;
                var culture = new CultureInfo("en-US");

                if (!DateTime.TryParseExact(timeOfDayUtc, "h:mm tt", culture, DateTimeStyles.AssumeUniversal,
                        out verifyTimeOfDay))
                {
                    await ModifyResponseAsync(
                        "The time of day could not be understood.",
                        ephemeral: true);
                    return;
                }
            }

            var zoneExists = await zoneRepository.GetByNameAsync(name);
            if (zoneExists == null)
            {
                // Create zone
                if (level == 0)
                {
                    await ModifyResponseAsync(
                        "Level cannot be 0 for a new record.",
                        ephemeral: true);
                    return;
                }

                if (dayOfWeekUtc == "")
                {
                    await ModifyResponseAsync(
                        "The day of the week cannot be blank for a new record.",
                        ephemeral: true);
                    return;
                }

                if (timeOfDayUtc == "")
                {
                    await ModifyResponseAsync(
                        "The time of day cannot be blank for a new record.",
                        ephemeral: true);
                    return;
                }

                Zone newZone = new(
                    0,
                    name,
                    level,
                    ownerAlliance,
                    dayOfWeekUtc.ToString(),
                    timeOfDayUtc,
                    notes
                );
                zoneRepository.Add(newZone);
                await zoneRepository.UnitOfWork.SaveEntitiesAsync();
                await zoneRepository.InitZones();
                await ModifyResponseAsync(
                    "Zone created",
                    ephemeral: true);
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
                    newDayOfWeek,
                    newTimeOfDay,
                    newNotes
                );
                zoneRepository.Update(zoneExists);
                await zoneRepository.UnitOfWork.SaveEntitiesAsync();
                await zoneRepository.InitZones();
                await ModifyResponseAsync(
                    "Zone updated",
                    ephemeral: true);
            }
        }
        catch (Exception ex)
        {
            await ModifyResponseAsync(
                "An unexpected error has occured.",
                ephemeral: true);
            _logger.LogError(ex,
                $"An unexpected error has occured while trying to run Zone Create Update for {Context.Guild.Name} in {Context.Channel.Name}.");
        }
    }

    /*
    [SlashCommand("connect","Bot Owner - Register a connection between two zones")]
    [RequireOwner]
    public async Task ConnectAsync(
        [Autocomplete(typeof(ZoneNames))] string zone1,
        [Autocomplete(typeof(ZoneNames))] string zone2)
    {
        using var serviceScope = _serviceProvider.CreateScope();
        try
        {
            var zoneRepository = serviceScope.ServiceProvider.GetService<IZoneRepository>();
            var objZone1 = await zoneRepository.GetByNameAsync(zone1);
            var objZone2 = await zoneRepository.GetByNameAsync(zone2);

            if (objZone1 == null)
            {
                await RespondAsync(
                    $"I'm sorry, I couldn't find a zone called {zone1}",
                    ephemeral: true);
                return;
            }

            if (objZone2 == null)
            {
                await RespondAsync(
                    $"I'm sorry, I couldn't find a zone called {zone2}",
                    ephemeral: true);
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

            await RespondAsync(
                results,
                ephemeral: true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                $"An unexpected error has occured while trying to run Connect for {Context.Guild.Name} in {Context.Channel.Name}.");
        }
    }
    */

    [SlashCommand("show", "Shows current zone info from the database")]
    public async Task ShowZoneAsync([Autocomplete(typeof(ZoneNames))] string name)
    {
        using var serviceScope = _serviceProvider.CreateScope();
        await this.DeferAsync(ephemeral: false);
        
        try
        {
            var zoneRepository = serviceScope.ServiceProvider.GetService<IZoneRepository>();
            var allianceRepository = serviceScope.ServiceProvider.GetService<IAllianceRepository>();
            
            // TODO: Get current alliance from Guild ID, then filter results.
            var thisAlliance = allianceRepository.FindFromGuildId(Context.Guild.Id);
            serviceScope.ServiceProvider.GetService<RequestContext>().Init(thisAlliance.Id);
            
            var thisZone = await zoneRepository.GetByNameAsync(name);
            if (thisZone == null)
            {
                await ModifyResponseAsync(
                    "Zone not found.",
                    ephemeral: true);
            }
            else
            {
                var potentialHostiles = zoneRepository
                    .GetContenders(thisZone.Id)
                    .Select(a => a.Acronym)
                    .OrderBy(a => a)
                    .ToList();
                var potentialThreats = "";
                potentialThreats = potentialHostiles.Any()
                    ? string.Join(", ", potentialHostiles)
                    : thisZone.Level == 1
                        ? "Anyone (1^)"
                        : "None";

                var owner = (thisZone.Owner == null ? "Unclaimed" : thisZone.Owner.Acronym);
                thisZone.SetNextDefend();
                string response =
                    $"Zone: {thisZone.Name} ({thisZone.Level}^)\n"
                    + $"Current Owner: {owner}\n"
                    + $"Next Event: <t:{thisZone.NextDefend.Value.ToUnixTimestamp()}> local / {thisZone.NextDefend.Value.ToEasternTime().ToString("h:mm tt")} ET\n"
                    + $"Contenders: {potentialThreats}\n";
                //if (!string.IsNullOrEmpty(thisZone.Threats))
                //    response += $"Saved Threats: {thisZone.Threats}\n";
                if (!string.IsNullOrEmpty(thisZone.Notes))
                    response += $"Notes: {thisZone.Notes}\n";
                await ModifyResponseAsync(response, ephemeral: false);
            }
        }
        catch (Exception ex)
        {
            await ModifyResponseAsync(
                "An unexpected error has occured.",
                ephemeral: true);
            _logger.LogError(ex,
                $"An unexpected error has occured while trying to run Show Zone for {Context.Guild.Name} in {Context.Channel.Name}.");
        }
    }
}