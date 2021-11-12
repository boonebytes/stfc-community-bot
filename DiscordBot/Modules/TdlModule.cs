using System;
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

        /*
        [Command("broadcast")]
        [Summary("Sends a broadcast message to registered servers")]
        public async Task BroadcastAsync([Remainder][Summary("The message to broadcast")] string message)
        {
            // Check to see if this is a guild user, which is the user context
            // where roles exist.
            if (Context.User is SocketGuildUser guildUser)
            {
                // Check for appropriate roles
                if (guildUser.Roles.Any(r => (new[] { "admin", "commodore", "commodores" }).Contains(r.Name.ToLower())))
                {
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                    _broadcast.SendBroadcast(Context.Channel, guildUser, message);
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                }
                else
                {
                    await this.ReplyAsync("Sorry, I couldn't find the right permissions for that.");
                }
            }
        }
        */
    }
}
