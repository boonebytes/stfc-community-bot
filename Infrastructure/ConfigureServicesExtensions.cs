using System;
using Amazon.Textract;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DiscordBot.Infrastructure
{
    public static class ConfigureServicesExtensions
    {
        public static void ConfigureBotInfrastructure(this IServiceCollection serviceCollection, string connectionString)
        {
            serviceCollection.AddDbContext<BotContext>(options =>
                    {
                        //options.UseMySql(connectionString);
                        options.UseOracle(connectionString);
                    },
                    ServiceLifetime.Scoped
                );

            serviceCollection.AddAWSService<IAmazonTextract>();
        }
    }
}
