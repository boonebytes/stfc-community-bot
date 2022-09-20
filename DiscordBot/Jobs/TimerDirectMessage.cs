using System.Data;
using Discord;
using Discord.WebSocket;
using DiscordBot.Domain.Entities.Alliances;
using DiscordBot.Domain.Entities.Request;
using DiscordBot.Domain.Entities.Zones;
using DiscordBot.Domain.Shared;
using Quartz;

namespace DiscordBot.Jobs;

[DisallowConcurrentExecution]
public class TimerDirectMessage : BaseJob
{
    private readonly DiscordSocketClient _client;
    
    public TimerDirectMessage(
        ILogger<TimerDirectMessage> logger,
        DiscordSocketClient client
        ) : base(logger)
    {
        _client = client;
    }
    
    protected override async Task DoWork(IJobExecutionContext context)
    {
        var data = context.JobDetail.JobDataMap;
        var userIdString = data.GetString("userId");
        var message = data.GetString("message");
        
        if (string.IsNullOrEmpty(userIdString))
            throw new NullReferenceException("User has not been set");

        ulong userId = 0;
        try
        {
            userId = Convert.ToUInt64(userIdString);
        }
        catch (Exception ex)
        {
            if (ex is FormatException or OverflowException)
            {
                Logger.LogError("User ID {UserId} could not be converted", userIdString);
                throw new DataException("User ID could not be converted");
            }
            throw;
        }

        await _client.GetUser(userId).SendMessageAsync($"Timer Reminder: {message}");
    }
}