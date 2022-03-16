using System.Configuration;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Json;
using Discord;
using Discord.WebSocket;
using DiscordBot.Infrastructure;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Web.Data;
using Web.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
//var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
//builder.Services.AddDbContext<ApplicationDbContext>(options =>
//    options.UseSqlite(connectionString));
//builder.Services.AddDatabaseDeveloperPageExceptionFilter();
//
//builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
//    .AddEntityFrameworkStores<ApplicationDbContext>();

IConfiguration configuration = builder.Configuration;

Web.Models.Config.Discord discordConfig = configuration.GetSection(Web.Models.Config.Discord.Section).Get<Web.Models.Config.Discord>();
builder.Services.AddSingleton(discordConfig);

Web.Models.Config.WebDb webDbConfig = configuration.GetSection(Web.Models.Config.WebDb.Section).Get<Web.Models.Config.WebDb>();
builder.Services.AddSingleton(webDbConfig);

builder.Services.ConfigureBotInfrastructure(configuration.GetSection("MySQL").GetValue<string>("ConnectionString"));

builder.Services.AddDbContext<WebDbContext>(options =>
    {
        options.UseMySql(webDbConfig.ConnectionString);
    },
    ServiceLifetime.Scoped);

builder.Services
    //.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    //.AddCookie(CookieAuthenticationDefaults.AuthenticationScheme,
    //    options =>
    //    {
    //        options.LoginPath = "/Account/Login";
    //        options.LogoutPath = "/Account/Logout";
    //    })
    .AddOAuth("Discord",
        options =>
        {
            var discordRoot = discordConfig.ApiRoot;
            options.AuthorizationEndpoint = $"{discordRoot}/oauth2/authorize";
            options.TokenEndpoint = $"{discordRoot}/oauth2/token";
            options.UserInformationEndpoint = $"{discordRoot}/users/@me";
            
            options.Scope.Add("identify");
            options.Scope.Add("guilds");

            options.CallbackPath = new PathString("/Account/DiscordReturn");
            options.ClientId = discordConfig.ClientId;
            options.ClientSecret = discordConfig.ClientSecret;
            
            options.ClaimActions.MapJsonKey(ClaimTypes.NameIdentifier, "id");
            options.ClaimActions.MapJsonKey(ClaimTypes.Name, "username");
            options.SaveTokens = true;
            
            options.Events = new OAuthEvents
            {
                OnCreatingTicket = async context =>
                {
                    // Get user info from the userinfo endpoint and use it to populate user claims
                    var request = new HttpRequestMessage(HttpMethod.Get, context.Options.UserInformationEndpoint);
                    request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", context.AccessToken);

                    var response = await context.Backchannel.SendAsync(
                        request,
                        HttpCompletionOption.ResponseHeadersRead,
                        context.HttpContext.RequestAborted);
                    response.EnsureSuccessStatusCode();

                    var user = JsonDocument.Parse(await response.Content.ReadAsStringAsync());

                    context.RunClaimActions(user.RootElement);

                    //if (context.AccessToken != null)
                    //    context.HttpContext.Session.SetString(SessionKeys.AccessToken, context.AccessToken);
                    //if (context.RefreshToken != null)
                    //    context.HttpContext.Session.SetString(SessionKeys.RefreshToken, context.RefreshToken);
                }
            };
        });

var clientConfig = new DiscordSocketConfig
{
    //ExclusiveBulkDelete = false,
    //AlwaysDownloadUsers = true,
    GatewayIntents =  GatewayIntents.AllUnprivileged
                      //| GatewayIntents.GuildMembers
                      //| GatewayIntents.GuildMessages
                      //| GatewayIntents.DirectMessages
};
//clientConfig.GatewayIntents &= Discord.GatewayIntents.GuildMembers;
var client = new DiscordSocketClient(clientConfig);
builder.Services.AddSingleton<DiscordSocketClient>(client);


builder.Services.AddAuthorization();

builder.Services
    .AddIdentity<AppUser, IdentityRole>()
    .AddEntityFrameworkStores<WebDbContext>();
//    .AddUserManager<>();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(60);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;

});

builder.Services
    .AddControllersWithViews()
    .AddSessionStateTempDataProvider();

builder.Services
    .AddRazorPages()
    .AddSessionStateTempDataProvider();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    //app.UseMigrationsEndPoint();
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseAuthentication();
app.UseAuthorization();
app.UseSession();

app.UseRouting();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

//app.UseEndpoints(endpoints =>
//{
//    endpoints.MapControllerRoute(
//        name: "default",
//        pattern: "{controller=Home}/{action=Index}/{id?}"
//    );
//    endpoints.MapControllerRoute(
//        name : "areas",
//        pattern : "{area:exists}/{controller=Home}/{action=Index}/{id?}"
//    );
//});

app.MapRazorPages();

app.Run();