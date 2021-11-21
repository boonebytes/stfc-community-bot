using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using DiscordBot.Domain.Entities.Alliances;
using DiscordBot.Domain.Entities.Zones;
using DiscordBot.Domain.Exceptions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace DiscordBot.Modules
{
    [Group("tdl")]
    public class TdlModule : ModuleBase<SocketCommandContext>
    {
        private readonly ILogger<TdlModule> _logger;
        private readonly IServiceProvider _serviceProvider;

        public TdlModule(ILogger<TdlModule> logger, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        protected async Task TryDeleteMessage(SocketUserMessage message)
        {
            try
            {
                await message.DeleteAsync();
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, $"Unable to delete source message.");
            }
        }

        [Command("help")]
        [Summary("Displays help information")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task HelpAsync()
        {
            await this.ReplyAsync("For support purposes, please conact Boonebytes.\n"
                            + "\n"
                            + "Accepted Commands:\n"
                            + "help - Displays this message\n"
                            + "today - Prints the defends scheduled for the remainder of today\n"
                            + "tomorrow - Prints the defend schedule for tomorrow\n"
                            + "next - Prints the next defend on the schedule");
        }

        [Command("today")]
        [Summary("Prints the defense times for the rest of today")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task TodayAsync(string extra = "")
        {
            using var serviceScope = _serviceProvider.CreateScope();
            try
            {
                var allianceRepository = serviceScope.ServiceProvider.GetService<IAllianceRepository>();
                var schedule = serviceScope.ServiceProvider.GetService<Responses.Schedule>();

                var thisAlliance = allianceRepository.FindFromGuildId(Context.Guild.Id);

                var shortVersion = false;
                if (extra.Trim().ToLower() == "short")
                    shortVersion = true;
                var embedMsg = schedule.GetForDate(DateTime.UtcNow, thisAlliance.Id, shortVersion);
                _ = TryDeleteMessage(Context.Message);
                await this.ReplyAsync(embed: embedMsg.Build());
            }
            catch (BotDomainException ex)
            {
                await this.ReplyAsync(ex.Message);
                _logger.LogError(ex, $"Exception when trying to run TODAY for {Context.Guild.Name} in {Context.Channel.Name}.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An unexpected error has occured while trying to run TODAY for {Context.Guild.Name} in {Context.Channel.Name}.");
            }
        }

        [Command("tomorrow")]
        [Summary("Prints the defense times for tomorrow")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task TomorrowAsync(string extra = "")
        {
            using var serviceScope = _serviceProvider.CreateScope();
            try
            {
                var allianceRepository = serviceScope.ServiceProvider.GetService<IAllianceRepository>();
                var schedule = serviceScope.ServiceProvider.GetService<Responses.Schedule>();

                var thisAlliance = allianceRepository.FindFromGuildId(Context.Guild.Id);

                var shortVersion = false;
                if (extra.Trim().ToLower() == "short")
                    shortVersion = true;

                var embedMsg = schedule.GetForDate(DateTime.UtcNow.AddDays(1), thisAlliance.Id, shortVersion);
                _ = TryDeleteMessage(Context.Message);
                await this.ReplyAsync(embed: embedMsg.Build());
            }
            catch (BotDomainException ex)
            {
                await this.ReplyAsync(ex.Message);
                _logger.LogError(ex, $"Exception when trying to run TOMORROW for {Context.Guild.Name} in {Context.Channel.Name}.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An unexpected error has occured while trying to run TOMORROW for {Context.Guild.Name} in {Context.Channel.Name}.");
            }
        }

        [Command("next")]
        [Summary("Prints the next item on the defend schedule")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task NextAsync()
        {
            using var serviceScope = _serviceProvider.CreateScope();
            try
            {
                var allianceRepository = serviceScope.ServiceProvider.GetService<IAllianceRepository>();
                var schedule = serviceScope.ServiceProvider.GetService<Responses.Schedule>();

                var thisAlliance = allianceRepository.FindFromGuildId(Context.Guild.Id);
                var embedMsg = schedule.GetNext(thisAlliance.Id);
                _ = TryDeleteMessage(Context.Message);
                await this.ReplyAsync(embed: embedMsg.Build());
            }
            catch (BotDomainException ex)
            {
                await this.ReplyAsync(ex.Message);
                _logger.LogError(ex, $"Exception when trying to run NEXT for {Context.Guild.Name} in {Context.Channel.Name}.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An unexpected error has occured while trying to run NEXT for {Context.Guild.Name} in {Context.Channel.Name}.");
            }
        }

        [Command("all", RunMode = RunMode.Async)]
        [Summary("Prints the full defense schedule")]
        [Alias("full")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task AllAsync(string extra = "")
        {
            using var serviceScope = _serviceProvider.CreateScope();
            try
            {
                var allianceRepository = serviceScope.ServiceProvider.GetService<IAllianceRepository>();
                var schedule = serviceScope.ServiceProvider.GetService<Responses.Schedule>();

                var thisAlliance = allianceRepository.FindFromGuildId(Context.Guild.Id);

                var shortVersion = false;
                if (extra.Trim().ToLower() == "short")
                    shortVersion = true;

                var targetGuild = Context.Guild.Id;
                var targetChannel = Context.Channel.Id;

                await schedule.PostAllAsync(targetGuild, targetChannel, thisAlliance.Id, shortVersion);
                await TryDeleteMessage(Context.Message);

                //await this.ReplyAsync(embed: embedMsg.Build());
            }
            catch (BotDomainException ex)
            {
                await this.ReplyAsync(ex.Message);
                _logger.LogError(ex, $"Exception when trying to run ALL for {Context.Guild.Name} in {Context.Channel.Name}.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An unexpected error has occured while trying to run ALL for {Context.Guild.Name} in {Context.Channel.Name}.");
            }
        }

        [Command("refresh", RunMode = RunMode.Async)]
        [Summary("Refreshes any short posts for the entire week")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task RefreshAsync()
        {
            using var serviceScope = _serviceProvider.CreateScope();
            try
            {
                var allianceRepository = serviceScope.ServiceProvider.GetService<IAllianceRepository>();
                var schedule = serviceScope.ServiceProvider.GetService<Responses.Schedule>();

                var thisAlliance = allianceRepository.FindFromGuildId(Context.Guild.Id);

                if (Context.Channel is SocketTextChannel channel)
                {
                    var channelMessages = await channel.GetMessagesAsync().FlattenAsync();

                    await schedule.TryCleanMessages(channel, channelMessages, thisAlliance);
                    await schedule.TryUpdateWeeklyMessages(channelMessages, thisAlliance);
                    await TryDeleteMessage(Context.Message);
                }
                else
                {
                    _logger.LogError($"Unable to cast context channel to text channel for {Context.Guild.Name} in {Context.Channel.Name}.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An unexpected error has occured while trying to run REFRESH for {Context.Guild.Name} in {Context.Channel.Name}.");
            }
        }

        [Command("alliance")]
        [Summary("Create or update an alliance")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task AllianceCreateUpdateAsync(string acronym, string name, string group = "")
        {
            using var serviceScope = _serviceProvider.CreateScope();
            try
            {
                var allianceRepository = serviceScope.ServiceProvider.GetService<IAllianceRepository>();

                var allianceExists = await allianceRepository.GetByNameOrAcronymAsync(acronym);
                if (allianceExists == null || allianceExists == default)
                {
                    allianceExists = await allianceRepository.GetByNameOrAcronymAsync(name);
                }


                AllianceGroup allianceGroup = null;
                if (group != "" && group != "0")
                {
                    var allianceGroups = allianceRepository.GetAllianceGroups();
                    allianceGroup = allianceGroups.FirstOrDefault(g => g.Name == group);
                }

                if (allianceExists == null || allianceExists == default)
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
                _logger.LogError(ex, $"An unexpected error has occured while trying to run Connect for {Context.Guild.Name} in {Context.Channel.Name}.");
            }
        }


        [Command("zone")]
        [Summary("Create or update a zone")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task ZoneCreateUpdateAsync(string name, string owner, int level = 0, string threats = "", string dayOfWeekUtc = "", string timeOfDayUtc = "", [Remainder] string notes = "")
        {
            using var serviceScope = _serviceProvider.CreateScope();
            try
            {
                var zoneRepository = serviceScope.ServiceProvider.GetService<IZoneRepository>();
                var allianceRepository = serviceScope.ServiceProvider.GetService<IAllianceRepository>();

                if (level < 0 || level > 3)
                {
                    await Context.Message.ReplyAsync("Level must be between 1 and 3. Specify 0 if you do not wish to change an existing value.");
                    return;
                }

                Alliance ownerAlliance = null;
                if (owner != "" && owner != "0")
                {
                    ownerAlliance = await allianceRepository.GetByNameOrAcronymAsync(owner);
                    if (ownerAlliance == null || ownerAlliance == default)
                    {
                        await Context.Message.ReplyAsync("The owner could not be found. Please check it and try again.");
                        return;
                    }
                }

                if (!(new[] { "", "Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday" }.Contains(dayOfWeekUtc)))
                {
                    await Context.Message.ReplyAsync("The day of the week could not be detrmined.");
                    return;
                }
                if (timeOfDayUtc != "")
                {
                    DateTime verifyTimeOfDay;
                    CultureInfo culture = new CultureInfo("en-US");

                    if (!DateTime.TryParseExact(timeOfDayUtc, "h:mm tt", culture, DateTimeStyles.AssumeUniversal, out verifyTimeOfDay))
                    {
                        await Context.Message.ReplyAsync("The time of day could not be understood.");
                        return;
                    }
                }

                if (notes == "\"\"") notes = "";

                var zoneExists = await zoneRepository.GetByNameAsync(name);
                if (zoneExists == null || zoneExists == default)
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
                    var newDayOfWeek = (dayOfWeekUtc == "" || dayOfWeekUtc == "0" ? zoneExists.DefendUtcDayOfWeek : dayOfWeekUtc);
                    var newTimeOfDay = (timeOfDayUtc == "" || timeOfDayUtc == "0" ? zoneExists.DefendUtcTime : timeOfDayUtc);

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
                _logger.LogError(ex, $"An unexpected error has occured while trying to run Zone Create Update for {Context.Guild.Name} in {Context.Channel.Name}.");
            }
        }

        [Command("connect")]
        [Summary("Register a connection between two zones")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task ConnectAsync(string zone1, string zone2)
        {
            using var serviceScope = _serviceProvider.CreateScope();
            try
            {
                var zoneRepository = serviceScope.ServiceProvider.GetService<IZoneRepository>();
                var objZone1 = await zoneRepository.GetByNameAsync(zone1);
                var objZone2 = await zoneRepository.GetByNameAsync(zone2);

                if (objZone1 == null || objZone1 == default)
                {
                    await Context.Message.ReplyAsync($"I'm sorry, I couldn't find a zone called {zone1}");
                    return;
                }
                if (objZone2 == null || objZone2 == default)
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
                _logger.LogError(ex, $"An unexpected error has occured while trying to run Connect for {Context.Guild.Name} in {Context.Channel.Name}.");
            }
        }

        [Command("show")]
        [Summary("Shows current info from the database")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task ShowAsync(string type, string name)
        {
            using var serviceScope = _serviceProvider.CreateScope();
            try
            {
                var zoneRepository = serviceScope.ServiceProvider.GetService<IZoneRepository>();
                var allianceRepository = serviceScope.ServiceProvider.GetService<IAllianceRepository>();

                switch (type.ToLower())
                {
                    case "zone":
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
                                .OrderBy(a => a);
                            var potentialThreats = "";
                            if (potentialHostiles.Count() > 0)
                            {
                                potentialThreats = string.Join(", ", potentialHostiles);
                            }
                            string response =
                                $"Zone : {thisZone.Name} ({thisZone.Level}^)\n"
                                + "Current Owner: " + (thisZone.Owner == null ? "Unclaimed" : thisZone.Owner.Acronym) + "\n"
                                + "Saved Threats: " + (string.IsNullOrEmpty(thisZone.Threats) ? "None" : thisZone.Threats) + "\n"
                                + "Potential Hostiles: " + (string.IsNullOrEmpty(potentialThreats) ? "None" : potentialThreats) + "\n"
                                + "Notes: " + (string.IsNullOrEmpty(thisZone.Notes) ? "None" : thisZone.Notes);
                            await ReplyAsync(response);
                        }
                        break;
                    case "alliance":
                        break;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An unexpected error has occured while trying to run Show for {Context.Guild.Name} in {Context.Channel.Name}.");
            }
        }
    }
}
