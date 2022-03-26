using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace DiscordBot.Infrastructure
{
    public class BotContextFactory : IDesignTimeDbContextFactory<BotContext>
    {
        public BotContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<BotContext>();
            optionsBuilder.UseOracle("Data Source=localhost:1521/XEPDB1;User Id=stfc_dev;Password=NotMyPassword;");
            

            return new BotContext(optionsBuilder.Options, null);
        }
    }
}
