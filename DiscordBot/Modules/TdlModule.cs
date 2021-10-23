using System;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;

namespace DiscordBot.Modules
{
    [Group("tdl")]
    public class TdlModule : ModuleBase<SocketCommandContext>
    {
        private readonly ILogger<TdlModule> _logger;
        private readonly Managers.DefendTimes _defendTimes;
        private readonly Managers.DiscordServers _discordServers;
        private readonly Responses.Schedule _schedule;
        private readonly Responses.Broadcast _broadcast;
        private readonly DiscordSocketClient _client;

        public TdlModule(ILogger<TdlModule> logger, Managers.DefendTimes defendTimes, Managers.DiscordServers discordServers, Responses.Schedule schedule, Responses.Broadcast broadcast, DiscordSocketClient client)
        {
            _logger = logger;
            _defendTimes = defendTimes;
            _discordServers = discordServers;
            _schedule = schedule;
            _broadcast = broadcast;
            _client = client;
        }

        [Command("today")]
        [Summary("Prints the defense times for today")]
        public async Task TodayAsync()
        {
            try
            {
                var embedMsg = _schedule.GetForDate(DateTime.UtcNow);
                await this.Context.Message.DeleteAsync();
                await this.ReplyAsync(embed: embedMsg.Build());
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
                var embedMsg = _schedule.GetForDate(DateTime.UtcNow.AddDays(1));
                await this.Context.Message.DeleteAsync();
                await this.ReplyAsync(embed: embedMsg.Build());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An unexpected error has occured while trying to run TOMORROW for {Context.Guild.Name} in {Context.Channel.Name}.");
            }
        }

        [Command("all")]
        [Summary("Prints the full defense schedule")]
        [Alias("full")]
        public async Task AllAsync()
        {
            try
            {
                var embedMsg = _schedule.GetAll();
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
