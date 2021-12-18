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
using DiscordBot.Domain.Shared;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace DiscordBot.Modules
{
    [Group("tdl")]
    public partial class TdlModule : ModuleBase<SocketCommandContext>
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
                _logger.LogWarning(ex, "Unable to delete source message");
            }
        }

        [Command("help")]
        [Summary("Displays help information")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task HelpAsync()
        {
            //TODO: Add new commands here
            await this.ReplyAsync("For support purposes, please contact Boonebytes.\n"
                            + "\n"
                            + "Accepted Commands:\n"
                            + "help - Displays this message\n"
                            + "today - Prints the defends scheduled for the remainder of today\n"
                            + "tomorrow - Prints the defend schedule for tomorrow\n"
                            + "next - Prints the next defend on the schedule");
        }

        [Command("echo")]
        [Summary("Repeats the message verbatim and then removes the original")]
        [RequireOwner]
        public async Task EchoAsync([Remainder] string message)
        {
            try
            {
                _ = TryDeleteMessage(Context.Message);
                await this.ReplyAsync(message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An unexpected error has occured while trying to run ECHO.");
            }
            
        }
        
        [Command("say")]
        [Summary("Posts the message verbatim to the specified channel and then removes the original")]
        [RequireOwner]
        public async Task SayAsync(ulong channelId, [Remainder] string message)
        {
            using var serviceScope = _serviceProvider.CreateScope();
            try
            {
                var client = serviceScope.ServiceProvider.GetService<DiscordSocketClient>();
                var channel = client.GetChannel(channelId);
                if (channel is Discord.ITextChannel textChannel)
                {
                    await textChannel.SendMessageAsync(message);
                    await TryDeleteMessage(Context.Message);
                }
                else
                {
                    await this.ReplyAsync("Unable to send message. Channel couldn't be resolved.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An unexpected error has occured while trying to run SAY for {channelId}.");
            }
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

                bool shortVersion = extra.Trim().ToLower() == "short";
                
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

                bool shortVersion = extra.Trim().ToLower() == "short";

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

                bool shortVersion = extra.Trim().ToLower() == "short";

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
                var zoneRepository = serviceScope.ServiceProvider.GetService<IZoneRepository>();
                var allianceRepository = serviceScope.ServiceProvider.GetService<IAllianceRepository>();
                var schedule = serviceScope.ServiceProvider.GetService<Responses.Schedule>();

                var thisAlliance = allianceRepository.FindFromGuildId(Context.Guild.Id);

                await zoneRepository.InitZones();
                if (Context.Channel is SocketTextChannel channel)
                {
                    var channelMessages = await channel.GetMessagesAsync().FlattenAsync();

                    await zoneRepository.InitZones(true);
                    //await schedule.TryCleanMessages(channel, channelMessages, thisAlliance);
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
    }
}
