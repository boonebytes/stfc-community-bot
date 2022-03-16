using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Web.Data;

public class WebDbContext : IdentityDbContext<Models.AppUser>
{
    private readonly ILogger<WebDbContext> _logger;
    
    public WebDbContext(DbContextOptions<WebDbContext> options, ILogger<WebDbContext> logger) : base(options)
    {
        _logger = logger;
    }
}