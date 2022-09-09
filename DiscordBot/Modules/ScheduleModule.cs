using System.Collections.Immutable;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using DiscordBot.AutocompleteHandlers;
using DiscordBot.Domain.Entities.Alliances;
using DiscordBot.Domain.Entities.Request;
using DiscordBot.Domain.Entities.Zones;
using DiscordBot.Domain.Exceptions;

namespace DiscordBot.Modules;

[Discord.Interactions.Group("schedule", "Admin Commands")]
public class ScheduleModule : BaseModule
{
    
    public ScheduleModule(ILogger<ScheduleModule> logger, IServiceProvider serviceProvider) : base(logger, serviceProvider)
    {
    }

    [SlashCommand("today", "Prints the defense times for the rest of today")]
    [RequireUserPermission(ChannelPermission.SendMessages)]
    public async Task TodayAsync(bool shortVersion = false)
    {
        using var serviceScope = _serviceProvider.CreateScope();
        try
        {
            _ = this.DeferAsync();
            var allianceRepository = serviceScope.ServiceProvider.GetService<IAllianceRepository>();
            var schedule = serviceScope.ServiceProvider.GetService<Responses.Schedule>();

            var thisAlliance = allianceRepository.FindFromGuildId(Context.Guild.Id);
            serviceScope.ServiceProvider.GetService<RequestContext>().Init(thisAlliance.Id);

            var embedMsg = schedule.GetForDate(DateTime.UtcNow, thisAlliance.Id, shortVersion);
            await this.ModifyResponseAsync(embed: embedMsg.Build(), ephemeral: false);
        }
        catch (BotDomainException ex)
        {
            await this.RespondAsync(ex.Message, ephemeral: true);
            _logger.LogError(ex, $"Exception when trying to run TODAY for {Context.Guild.Name} in {Context.Channel.Name}.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"An unexpected error has occured while trying to run TODAY for {Context.Guild.Name} in {Context.Channel.Name}.");
            await RespondAsync(
                "An unexpected error has occured.",
                ephemeral: true);
        }
    }
        
    [SlashCommand("tomorrow","Prints the defense times for tomorrow")]
    [RequireUserPermission(ChannelPermission.SendMessages)]
    public async Task TomorrowAsync(bool shortVersion = false)
    {
        using var serviceScope = _serviceProvider.CreateScope();
        try
        {
            _ = this.DeferAsync();
            var allianceRepository = serviceScope.ServiceProvider.GetService<IAllianceRepository>();
            var schedule = serviceScope.ServiceProvider.GetService<Responses.Schedule>();

            var thisAlliance = allianceRepository.FindFromGuildId(Context.Guild.Id);
            serviceScope.ServiceProvider.GetService<RequestContext>().Init(thisAlliance.Id);

            var embedMsg = schedule.GetForDate(DateTime.UtcNow.AddDays(1), thisAlliance.Id, shortVersion);
            await this.ModifyResponseAsync(embed: embedMsg.Build(), ephemeral: false);
        }
        catch (BotDomainException ex)
        {
            await this.RespondAsync(
                ex.Message,
                ephemeral: true);
            _logger.LogError(ex, $"Exception when trying to run TOMORROW for {Context.Guild.Name} in {Context.Channel.Name}.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"An unexpected error has occured while trying to run TOMORROW for {Context.Guild.Name} in {Context.Channel.Name}.");
            await RespondAsync(
                "An unexpected error has occured.",
                ephemeral: true);
        }
    }

    [SlashCommand("next","Prints the next item on the defend schedule")]
    [RequireUserPermission(ChannelPermission.SendMessages)]
    public async Task NextAsync()
    {
        using var serviceScope = _serviceProvider.CreateScope();
        try
        {
            _ = this.DeferAsync(true);
            var allianceRepository = serviceScope.ServiceProvider.GetService<IAllianceRepository>();
            var schedule = serviceScope.ServiceProvider.GetService<Responses.Schedule>();

            var thisAlliance = allianceRepository.FindFromGuildId(Context.Guild.Id);
            serviceScope.ServiceProvider.GetService<RequestContext>().Init(thisAlliance.Id);
            
            var embedMsg = schedule.GetNext(thisAlliance.Id);
            _ = this.DeleteOriginalResponseAsync();
            await this.Context.Channel.SendMessageAsync(embed: embedMsg.Build());
        }
        catch (BotDomainException ex)
        {
            await this.RespondAsync(ex.Message, ephemeral: true);
            _logger.LogError(ex, $"Exception when trying to run NEXT for {Context.Guild.Name} in {Context.Channel.Name}.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"An unexpected error has occured while trying to run NEXT for {Context.Guild.Name} in {Context.Channel.Name}.");
            await RespondAsync(
                "An unexpected error has occured.",
                ephemeral: true);
        }
    }

    [SlashCommand("all", "Prints the full defense schedule", runMode: RunMode.Async)]
    [RequireUserPermission(ChannelPermission.SendMessages)]
    public async Task AllAsync(bool shortVersion = false)
    {
        using var serviceScope = _serviceProvider.CreateScope();
        try
        {
            await this.DeferAsync(ephemeral: true);
            var allianceRepository = serviceScope.ServiceProvider.GetService<IAllianceRepository>();
            var thisAlliance = allianceRepository.FindFromGuildId(Context.Guild.Id);
            serviceScope.ServiceProvider.GetService<RequestContext>().Init(thisAlliance.Id);

            var schedule = serviceScope.ServiceProvider.GetService<Responses.Schedule>();
            
            var targetGuild = Context.Guild.Id;
            var targetChannel = Context.Channel.Id;

            await schedule.PostAllAsync(targetGuild, targetChannel, thisAlliance.Id, shortVersion);
            await DeleteOriginalResponseAsync();
        }
        catch (BotDomainException ex)
        {
            await this.RespondAsync(
                ex.Message,
                ephemeral: true);
            _logger.LogError(ex, $"Exception when trying to run ALL for {Context.Guild.Name} in {Context.Channel.Name}.");
        }
        catch (Exception ex)
        {
            await RespondAsync(
                "An unexpected error has occured.",
                ephemeral: true);
            _logger.LogError(ex, $"An unexpected error has occured while trying to run ALL for {Context.Guild.Name} in {Context.Channel.Name}.");
        }
    }

    [SlashCommand("refresh", "Refreshes any short posts for the entire week", runMode: RunMode.Async)]
    [RequireUserPermission(GuildPermission.ManageGuild, Group = "Permission")]
    [RequireOwner(Group = "Permission")]
    public async Task RefreshAsync()
    {
        using var serviceScope = _serviceProvider.CreateScope();
        await this.DeferAsync(ephemeral: true);
        try
        {
            var zoneRepository = serviceScope.ServiceProvider.GetService<IZoneRepository>();
            var allianceRepository = serviceScope.ServiceProvider.GetService<IAllianceRepository>();
            var schedule = serviceScope.ServiceProvider.GetService<Responses.Schedule>();

            var thisAlliance = allianceRepository.FindFromGuildId(Context.Guild.Id);
            serviceScope.ServiceProvider.GetService<RequestContext>().Init(thisAlliance.Id);
            
            if (Context.Channel is SocketTextChannel channel)
            {
                var channelMessages = await channel.GetMessagesAsync().FlattenAsync();

                await zoneRepository.InitZones(true);
                await schedule.TryUpdateWeeklyMessages(channelMessages, thisAlliance);
                await ModifyResponseAsync("Refresh complete", true);
            }
            else
            {
                _logger.LogError($"Unable to cast context channel to text channel for {Context.Guild.Name} in {Context.Channel.Name}.");
                await ModifyResponseAsync(
                    "An unexpected error has occured.",
                    ephemeral: true);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"An unexpected error has occured while trying to run REFRESH for {Context.Guild.Name} in {Context.Channel.Name}.");
            await ModifyResponseAsync(
                "An unexpected error has occured.",
                ephemeral: true);
        }
    }
}