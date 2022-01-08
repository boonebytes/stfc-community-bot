using Discord;
using Discord.Commands;
using Discord.Interactions;
using Discord.WebSocket;
using DiscordBot.Domain.Entities.Admin;
using DiscordBot.Domain.Entities.Alliances;
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

                services.ConfigureBotInfrastructure(configuration.GetSection("MySQL").GetValue<string>("ConnectionString"));

                services.AddScoped<IAllianceRepository, AllianceRepository>();
                services.AddScoped<IDirectMessageRepository, DirectMessageRepository>();
                services.AddScoped<IZoneRepository, ZoneRepository>();
                services.AddScoped<IServiceRepository, ServiceRepository>();

                services.AddSingleton(discordConfig);
                var clientConfig = new DiscordSocketConfig
                {
                    //ExclusiveBulkDelete = false,
                    AlwaysDownloadUsers = true,
                    GatewayIntents =  GatewayIntents.AllUnprivileged
                                      | GatewayIntents.GuildMembers
                                      | GatewayIntents.GuildMessages
                                      | GatewayIntents.DirectMessages
                };
                //clientConfig.GatewayIntents &= Discord.GatewayIntents.GuildMembers;
                var client = new DiscordSocketClient(clientConfig);
                services.AddSingleton<DiscordSocketClient>(client);

                    
                var interactionService = new InteractionService(client.Rest);
                services.AddSingleton<InteractionService>(interactionService);
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