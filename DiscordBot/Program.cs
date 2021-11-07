using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord.Commands;
using Discord.WebSocket;
using DiscordBot.Domain.Entities.Alliances;
using DiscordBot.Domain.Entities.Zones;
using DiscordBot.Infrastructure;
using DiscordBot.Infrastructure.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace DiscordBot
{
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
                    /*
                    services.AddLogging(opt =>
                    {
                        opt.AddConsole(c =>
                        {
                            c.TimestampFormat = "[HH:mm:ss]";
                        });
                    });
                    */

                    IConfiguration configuration = hostContext.Configuration;
                    Models.Config.Discord discordConfig = configuration.GetSection(Models.Config.Discord.SECTION).Get<Models.Config.Discord>();

                    services.ConfigureBotInfrastructure(configuration.GetSection("MySQL").GetValue<string>("ConnectionString"));

                    services.AddScoped<IZoneRepository, ZoneRepository>();
                    services.AddScoped<IAllianceRepository, AllianceRepository>();

                    //CommandService commandService = new CommandService();

                    services.AddSingleton(discordConfig);

                    var clientConfig = new DiscordSocketConfig
                    {
                        ExclusiveBulkDelete = false,
                        //GatewayIntents =  Discord.GatewayIntents.GuildMembers
                        //                & Discord.GatewayIntents.GuildMessages
                    };
                    var client = new DiscordSocketClient(clientConfig);
                    services.AddSingleton<DiscordSocketClient>(client);

                    services.AddSingleton<CommandService>();
                    services.AddSingleton<Services.CommandHandler>();

                    services.AddTransient<Responses.Schedule>();
                    services.AddTransient<Responses.Broadcast>();

                    services.AddSingleton<Scheduler>();
                    services.AddHostedService<Worker>();

                    services.BuildServiceProvider();
                });
    }
}
