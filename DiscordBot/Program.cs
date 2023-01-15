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
using Discord.Commands;
using Discord.Interactions;
using Discord.WebSocket;
using DiscordBot.Domain.Entities.Admin;
using DiscordBot.Domain.Entities.Alliances;
using DiscordBot.Domain.Entities.Request;
using DiscordBot.Domain.Entities.Services;
using DiscordBot.Domain.Entities.Zones;
using DiscordBot.Infrastructure;
using DiscordBot.Infrastructure.Repositories;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Quartz;

namespace DiscordBot;

public class Program
{
    //protected static HttpClientGlobalListener observer = new HttpClientGlobalListener();
    //protected static IDisposable subscription = null; 

    public static void Main(string[] args)
    {
        //subscription = DiagnosticListener.AllListeners.Subscribe(observer);

        CreateHostBuilder(args).Build().Run();
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureLogging(builder =>
            {
                builder.AddFilter((provider, category, logLevel) =>
                {
                    if (category.Contains("Microsoft.EntityFrameworkCore.Model.Validation"))
                        return false;
                    if (category.Contains("Quartz.Core.QuartzSchedulerThread"))
                        return false;
                    
                    return true;
                });
            })
            .ConfigureAppConfiguration((hostContext, builder) =>
            {
                if (hostContext.HostingEnvironment.IsDevelopment())
                {
                    builder.AddUserSecrets<Worker>();
                }
            })
            .ConfigureServices((hostContext, services) =>
            {
                IConfiguration configuration = hostContext.Configuration;
                Models.Config.Discord discordConfig = configuration.GetSection(Models.Config.Discord.Section).Get<Models.Config.Discord>();

                Models.Config.App appConfig = configuration.GetSection(Models.Config.App.Section).Get<Models.Config.App>();
                
                //services.ConfigureBotInfrastructure(configuration.GetSection("MySQL").GetValue<string>("ConnectionString"));
                services.ConfigureBotInfrastructure(configuration.GetSection("Oracle").GetValue<string>("ConnectionString"));

                services.AddScoped<RequestContext>();
                
                services.AddScoped<IAllianceRepository, AllianceRepository>();
                services.AddScoped<ICustomMessageJobRepository, CustomMessageJobRepository>();
                services.AddScoped<IDirectMessageRepository, DirectMessageRepository>();
                services.AddScoped<IZoneRepository, ZoneRepository>();
                services.AddScoped<IServiceRepository, ServiceRepository>();
                services.AddScoped<IReactMessageRepository, ReactMessageRepository>();

                services.AddSingleton(discordConfig);
                services.AddSingleton(appConfig);
                
                var clientConfig = new DiscordSocketConfig
                {
                    //ExclusiveBulkDelete = false,
                    AlwaysDownloadUsers = true,
                    GatewayIntents =
                        // All Unprivileged = 0001 0111 1110 1111 1101
                        // 1, 4, 8, 16, 32, 64, 128, 512, 1024, 2048, 4096, 8192, 16384, 65536
                        // WAS:
                        // GatewayIntents =  GatewayIntents.AllUnprivileged
                        //                   | GatewayIntents.GuildMembers
                        //                   | GatewayIntents.GuildMessages
                        //                   | GatewayIntents.DirectMessages,
                        GatewayIntents.Guilds // 1
                        | GatewayIntents.GuildMembers // 2
                        // | GatewayIntents.GuildBans // 4
                        // | GatewayIntents.GuildEmojis // 8
                        | GatewayIntents.GuildIntegrations // 16
                        | GatewayIntents.GuildWebhooks // 32
                        // | GatewayIntents.GuildInvites // 64
                        // | GatewayIntents.GuildVoiceStates // 128
                        | GatewayIntents.GuildMessages // 512
                        // | GatewayIntents.GuildMessageReactions // 1024
                        // | GatewayIntents.GuildMessageTyping // 2048
                        | GatewayIntents.DirectMessages // 4096
                        // | GatewayIntents.DirectMessageReactions // 8192
                        // | GatewayIntents.DirectMessageTyping // 16384
                        | GatewayIntents.MessageContent // 32768
                        // | GatewayIntents.GuildScheduledEvents // 655536
                        ,
                    MessageCacheSize = 100
                };
                //clientConfig.GatewayIntents &= Discord.GatewayIntents.GuildMembers;
                var discordClient = new DiscordSocketClient(clientConfig);
                services.AddSingleton(discordClient);

                    
                services.AddSingleton<Services.InteractionHandler>();
                    

                services.AddSingleton<CommandService>();
                services.AddSingleton<Services.CommandHandler>();
                    
                services.AddTransient<Responses.Schedule>();

                services.AddSingleton<Scheduler>();
                services.AddHostedService<Worker>();

                services.AddMediatR(
                    typeof(Scheduler).Assembly,
                    typeof(Domain.Events.AllianceUpdatedDomainEvent).Assembly);

                services.AddQuartz(q =>
                {
                    q.UseMicrosoftDependencyInjectionJobFactory();
                });

                services.AddQuartzHostedService(q =>
                {
                    q.WaitForJobsToComplete = true;
                });

                services.BuildServiceProvider();
            });
}