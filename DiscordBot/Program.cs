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
                
                //services.ConfigureBotInfrastructure(configuration.GetSection("MySQL").GetValue<string>("ConnectionString"));
                services.ConfigureBotInfrastructure(configuration.GetSection("Oracle").GetValue<string>("ConnectionString"));

                services.AddScoped<RequestContext>();
                
                services.AddScoped<IAllianceRepository, AllianceRepository>();
                services.AddScoped<IDirectMessageRepository, DirectMessageRepository>();
                services.AddScoped<IZoneRepository, ZoneRepository>();
                services.AddScoped<IServiceRepository, ServiceRepository>();

                services.AddSingleton(discordConfig);
                services.AddSingleton(appConfig);
                
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
                    q.UseMicrosoftDependencyInjectionJobFactory();
                });

                services.AddQuartzHostedService(q =>
                {
                    q.WaitForJobsToComplete = true;
                });

                services.BuildServiceProvider();
            });
}