using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Web.Data
{
    public class WebDbContextFactory : IDesignTimeDbContextFactory<WebDbContext>
    {
        public WebDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<WebDbContext>();
            optionsBuilder.UseMySql("server=localhost; port=3306; database=discord_tdl_dev_users; user=tdlbot_dev_users; password=Password; Persist Security Info=False; Connect Timeout=300");

            return new WebDbContext(optionsBuilder.Options, null);
        }
    }
}
