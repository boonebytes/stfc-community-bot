using Microsoft.EntityFrameworkCore;
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
                        
                        // TODO: Investigate query splitting behavior
                        // (src: https://learn.microsoft.com/en-gb/ef/core/querying/single-split-queries)
                        options.UseOracle(connectionString, oracleOptions =>
                        {
                            //oracleOptions.UseQuerySplittingBehavior(QuerySplittingBehavior.SingleQuery);
                        });
                    },
                    ServiceLifetime.Scoped
                );
        }
    }
}
