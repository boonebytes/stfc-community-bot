using System;
using System.Threading.Tasks;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using DiscordBot.Domain.Entities.Alliances;
using DiscordBot.Domain.Entities.Zones;
using DiscordBot.Domain.Exceptions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace DiscordBot.Modules
{
    [Discord.Interactions.Group("stfc", "Star Trek Fleet Command - Community Bot")]
    public partial class StfcModule : InteractionModuleBase
    {
        private readonly ILogger<StfcModule> _logger;
        private readonly IServiceProvider _serviceProvider;

        public StfcModule(ILogger<StfcModule> logger, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }
        
        [RequireOwner]
        [SlashCommand("echo", "Echo text back to this channel")]
        public async Task EchoAsync(
            [Summary("Input", "Text to repeat")] string input)
        {
            if (Context.Channel is ISocketMessageChannel channel)
            {
                await channel.SendMessageAsync(input);
                await RespondAsync("Done!", ephemeral: true);
            }
            else
            {
                await RespondAsync("I can't do that; this doesn't look like a message channel.", ephemeral: true);
            }
        }
        
        [SlashCommand("today", "Prints the defense times for the rest of today")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task TodayAsync(bool shortVersion = false)
        {
            using var serviceScope = _serviceProvider.CreateScope();
            try
            {
                var allianceRepository = serviceScope.ServiceProvider.GetService<IAllianceRepository>();
                var schedule = serviceScope.ServiceProvider.GetService<Responses.Schedule>();

                var thisAlliance = allianceRepository.FindFromGuildId(Context.Guild.Id);

                var embedMsg = schedule.GetForDate(DateTime.UtcNow, thisAlliance.Id, shortVersion);
                //_ = TryDeleteMessage(Context.Message);
                await this.RespondAsync(embed: embedMsg.Build());
            }
            catch (BotDomainException ex)
            {
                await this.RespondAsync(ex.Message, ephemeral: true);
                _logger.LogError(ex, $"Exception when trying to run TODAY for {Context.Guild.Name} in {Context.Channel.Name}.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An unexpected error has occured while trying to run TODAY for {Context.Guild.Name} in {Context.Channel.Name}.");
            }
        }
        
        [SlashCommand("tomorrow","Prints the defense times for tomorrow")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task TomorrowAsync(bool shortVersion = false)
        {
            using var serviceScope = _serviceProvider.CreateScope();
            try
            {
                var allianceRepository = serviceScope.ServiceProvider.GetService<IAllianceRepository>();
                var schedule = serviceScope.ServiceProvider.GetService<Responses.Schedule>();

                var thisAlliance = allianceRepository.FindFromGuildId(Context.Guild.Id);

                var embedMsg = schedule.GetForDate(DateTime.UtcNow.AddDays(1), thisAlliance.Id, shortVersion);
                //_ = TryDeleteMessage(Context.Message);
                await this.RespondAsync(embed: embedMsg.Build());
            }
            catch (BotDomainException ex)
            {
                await this.RespondAsync(ex.Message, ephemeral: true);
                _logger.LogError(ex, $"Exception when trying to run TOMORROW for {Context.Guild.Name} in {Context.Channel.Name}.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An unexpected error has occured while trying to run TOMORROW for {Context.Guild.Name} in {Context.Channel.Name}.");
            }
        }

        [SlashCommand("next","Prints the next item on the defend schedule")]
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
                //_ = TryDeleteMessage(Context.Message);
                await this.RespondAsync(embed: embedMsg.Build());
            }
            catch (BotDomainException ex)
            {
                await this.RespondAsync(ex.Message, ephemeral: true);
                _logger.LogError(ex, $"Exception when trying to run NEXT for {Context.Guild.Name} in {Context.Channel.Name}.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An unexpected error has occured while trying to run NEXT for {Context.Guild.Name} in {Context.Channel.Name}.");
            }
        }

        [SlashCommand("all", "Prints the full defense schedule", runMode: RunMode.Async)]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task AllAsync(bool shortVersion = false)
        {
            using var serviceScope = _serviceProvider.CreateScope();
            try
            {
                var allianceRepository = serviceScope.ServiceProvider.GetService<IAllianceRepository>();
                var schedule = serviceScope.ServiceProvider.GetService<Responses.Schedule>();

                var thisAlliance = allianceRepository.FindFromGuildId(Context.Guild.Id);

                var targetGuild = Context.Guild.Id;
                var targetChannel = Context.Channel.Id;

                await schedule.PostAllAsync(targetGuild, targetChannel, thisAlliance.Id, shortVersion);
                //await TryDeleteMessage(Context.Message);
                await DeleteOriginalResponseAsync();

                //await this.ReplyAsync(embed: embedMsg.Build());
            }
            catch (BotDomainException ex)
            {
                await this.RespondAsync(ex.Message, ephemeral: true);
                _logger.LogError(ex, $"Exception when trying to run ALL for {Context.Guild.Name} in {Context.Channel.Name}.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An unexpected error has occured while trying to run ALL for {Context.Guild.Name} in {Context.Channel.Name}.");
            }
        }

        [SlashCommand("refresh", "Refreshes any short posts for the entire week", runMode: RunMode.Async)]
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
                    //await TryDeleteMessage(Context.Message);
                    await DeleteOriginalResponseAsync();
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