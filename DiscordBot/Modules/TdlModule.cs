using System;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using DiscordBot.Domain.Entities.Alliances;
using DiscordBot.Domain.Entities.Zones;
using DiscordBot.Domain.Exceptions;
using Microsoft.Extensions.Logging;

namespace DiscordBot.Modules
{
    [Group("tdl")]
    public class TdlModule : ModuleBase<SocketCommandContext>
    {
        private readonly ILogger<TdlModule> _logger;
        private readonly IZoneRepository _zoneRepository;
        private readonly IAllianceRepository _allianceRepository;
        private readonly Responses.Schedule _schedule;
        private readonly Responses.Broadcast _broadcast;
        private readonly DiscordSocketClient _client;

        public TdlModule(ILogger<TdlModule> logger, IZoneRepository zoneRepository, IAllianceRepository allianceRepository, Responses.Schedule schedule, Responses.Broadcast broadcast, DiscordSocketClient client)
        {
            _logger = logger;
            _zoneRepository = zoneRepository;
            _allianceRepository = allianceRepository;
            _schedule = schedule;
            _broadcast = broadcast;
            _client = client;
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
        public async Task TodayAsync()
        {
            try
            {
                var thisAlliance = _allianceRepository.FindFromGuildId(Context.Guild.Id);
                var embedMsg = _schedule.GetForDate(DateTime.UtcNow, thisAlliance.Id);
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
        public async Task TomorrowAsync()
        {
            try
            {
                var thisAlliance = _allianceRepository.FindFromGuildId(Context.Guild.Id);
                var embedMsg = _schedule.GetForDate(DateTime.UtcNow.AddDays(1), thisAlliance.Id);
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
            try
            {
                var thisAlliance = _allianceRepository.FindFromGuildId(Context.Guild.Id);
                var embedMsg = _schedule.GetNext(thisAlliance.Id);
                _ = TryDeleteMessage(Context.Message);
                await this.ReplyAsync(embed: embedMsg.Build());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An unexpected error has occured while trying to run NEXT for {Context.Guild.Name} in {Context.Channel.Name}.");
            }
        }

        [Command("all")]
        [Summary("Prints the full defense schedule")]
        [Alias("full")]
        public async Task AllAsync()
        {
            try
            {
                //TODO: Look at restricting this to just the interested alliances.
                var embedMsg = await _schedule.GetAll();
                _ = TryDeleteMessage(Context.Message);
                await this.ReplyAsync(embed: embedMsg.Build());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An unexpected error has occured while trying to run ALL for {Context.Guild.Name} in {Context.Channel.Name}.");
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
