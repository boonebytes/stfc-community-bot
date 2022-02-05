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
            optionsBuilder.UseMySql("server=localhost; port=3306; database=discord_tdl; user=tdlbot; password=Pa$$w0rd; Persist Security Info=False; Connect Timeout=300");

            return new BotContext(optionsBuilder.Options, null);
        }
    }
}
