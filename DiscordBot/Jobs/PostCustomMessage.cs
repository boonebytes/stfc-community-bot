/*
Copyright 2022 Boonebytes

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/

using Discord;
using Discord.WebSocket;
using DiscordBot.Domain.Entities.Admin;
using DiscordBot.Domain.Entities.Request;
using Quartz;

namespace DiscordBot.Jobs;

[DisallowConcurrentExecution]
public class PostCustomMessage : BaseJob
{
    public const string DATAMAP_ALLIANCE_ID = "allianceId";
    public const string DATAMAP_JOB_ID = "itemId";

    private readonly DiscordSocketClient _client;

    private ICustomMessageJobRepository _customMessageJobRepository;
    private RequestContext _requestContext;

    public PostCustomMessage(
        ILogger<PostCustomMessage> logger,
        DiscordSocketClient client,
        ICustomMessageJobRepository customMessageJobRepository,
        RequestContext requestContext
        ) : base(logger)
    {
        _client = client;
        _customMessageJobRepository = customMessageJobRepository;
        _requestContext = requestContext;
    }
    
    protected override async Task DoWork(IJobExecutionContext context)
    {
        var data = context.JobDetail.JobDataMap;
        var allianceId = data.GetLong(DATAMAP_ALLIANCE_ID);
        if (allianceId == 0)
            throw new NullReferenceException("Alliance has not been set");

        var jobId = data.GetLong(DATAMAP_JOB_ID);
        if (jobId == 0)
            throw new NullReferenceException("Job ID has not been set");

        var job = await _customMessageJobRepository.GetAsync(jobId);
        if (job == default)
            throw new NullReferenceException("Job not found");
        if (job.Status != JobStatus.Scheduled)
            throw new NullReferenceException("Job not in Scheduled status");
        
        var channelId = job.ChannelId;
        if (channelId == 0)
            throw new NullReferenceException("Channel ID has not been set");

        _requestContext.Init(allianceId);
        
        var alliance = job.Alliance;
        if (alliance == null)
            throw new KeyNotFoundException("The alliance could not be found");
        if (!alliance.GuildId.HasValue)
            throw new InvalidOperationException("Alliance doesn't have a guild");
        
        var guild = _client.GetGuild(alliance.GuildId.Value);
        if (guild == null)
            throw new InvalidOperationException("Unable to access guild");
        
        var channel = guild.GetTextChannel(channelId);
        if (channel == null)
            throw new InvalidOperationException("Unable to access channel");

        var message = job.Message;

        try
        {
            await channel.SendMessageAsync(message, allowedMentions: AllowedMentions.All);
            job.MarkCompleted();
        }
        catch (Exception e)
        {
            job.MarkFailed();
        }
        finally
        {
            _customMessageJobRepository.Update(job);
            await _customMessageJobRepository.UnitOfWork.SaveEntitiesAsync();
        }
    }
}