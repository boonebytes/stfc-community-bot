using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

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
                .ConfigureServices((hostContext, services) =>
                {
                    IConfiguration configuration = hostContext.Configuration;
                    Models.Config.Discord discordConfig = configuration.GetSection(Models.Config.Discord.SECTION).Get<Models.Config.Discord>();

                    //CommandService commandService = new CommandService();

                    services.AddSingleton(discordConfig);

                    services.AddSingleton<Managers.DefendTimes>();
                    services.AddSingleton<Managers.DiscordServers>();

                    services.AddSingleton<DiscordSocketClient>();
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
