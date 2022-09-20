﻿using Discord;
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
using DiscordBot.Jobs;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Quartz;

namespace DiscordBot;

public class Program
{
    public static void Main(string[] args)
    {
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
                Models.Config.Scheduler schedulerConfig = configuration.GetSection(Models.Config.Scheduler.Section).Get<Models.Config.Scheduler>();
                
                //services.ConfigureBotInfrastructure(configuration.GetSection("MySQL").GetValue<string>("ConnectionString"));
                services.ConfigureBotInfrastructure(configuration.GetSection("Oracle").GetValue<string>("ConnectionString"));

                services.AddScoped<RequestContext>();
                
                services.AddScoped<IAllianceRepository, AllianceRepository>();
                services.AddScoped<IDirectMessageRepository, DirectMessageRepository>();
                services.AddScoped<IZoneRepository, ZoneRepository>();
                services.AddScoped<IServiceRepository, ServiceRepository>();

                services.AddSingleton(discordConfig);
                services.AddSingleton(appConfig);
                services.AddSingleton(schedulerConfig);

                var clientConfig = new DiscordSocketConfig
                {
                    //ExclusiveBulkDelete = false,
                    AlwaysDownloadUsers = true,
                    GatewayIntents =  GatewayIntents.AllUnprivileged
                                      | GatewayIntents.GuildMembers
                                      | GatewayIntents.GuildMessages
                                      | GatewayIntents.DirectMessages,
                    MessageCacheSize = 100
                };
                //clientConfig.GatewayIntents &= Discord.GatewayIntents.GuildMembers;
                var discordClient = new DiscordSocketClient(clientConfig);
                services.AddSingleton(discordClient);

                    
                var interactionService = new InteractionService(discordClient.Rest);
                services.AddSingleton(interactionService);
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
                    //q.SchedulerName = schedulerConfig.RamName;
                    q.UseMicrosoftDependencyInjectionJobFactory();
                    q.UseDefaultThreadPool(x => x.MaxConcurrency = schedulerConfig.RamMaxConcurrency);
                    q.UseInMemoryStore();
                });

                // Only add jobs that go into persistent storage here.
                // RAM stored jobs are loaded fine with the above AddQuartz block
                services.AddTransient<TimerDirectMessage>();
                
                /*
                services.AddQuartz(q =>
                {
                    q.SchedulerName = schedulerConfig.PersistentName;
                    q.UseMicrosoftDependencyInjectionJobFactory();
                    q.UseDedicatedThreadPool(x => x.MaxConcurrency = schedulerConfig.PersistentMaxConcurrency);
                    q.UsePersistentStore(x =>
                    {
                        // force job data map values to be considered as strings
                        // prevents nasty surprises if object is accidentally serialized and then 
                        // serialization format breaks, defaults to false
                        x.UseProperties = true;

                        x.UseOracle(schedulerConfig.PersistentDbConnection);

                        // this requires Quartz.Serialization.Json NuGet package
                        x.UseJsonSerializer();
                    });
                });
                */
                
                /*
                services.AddQuartzHostedService(q =>
                {
                    q.WaitForJobsToComplete = true;
                });
                */

                services.BuildServiceProvider();
            });
}