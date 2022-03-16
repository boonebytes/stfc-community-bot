using System.Diagnostics;
using System.Net.Http.Headers;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RestSharp;
using RestSharp.Authenticators.OAuth2;
using Web.Models;

namespace Web.Controllers;

public class HomeController : BaseController
{
    private readonly Web.Models.Config.Discord discordConfig;

    public HomeController(ILogger<HomeController> logger, SignInManager<AppUser> signInManager, Models.Config.Discord discordConfig) : base(logger, signInManager)
    {
        this.discordConfig = discordConfig;
    }

    [AllowAnonymous]
    public async Task<IActionResult> Index()
    {
        if (SignInManager.IsSignedIn(User))
        {
            //var info = await SignInManager.GetExternalLoginInfoAsync();
            
            //if (info == null)
            //{
            //    ControllerContext.HttpContext.Session.Clear();
            //}
            //var accessToken = info.AuthenticationTokens.Single(t => t.Name == "access_token").Value;
            var accessToken = await HttpContext.GetTokenAsync("access_token");

            var client = new RestClient(discordConfig.ApiRoot + "/");
            client.Authenticator = new OAuth2AuthorizationRequestHeaderAuthenticator(accessToken, "Bearer");
            var request = new RestRequest("users/@me/guilds");
            var response = await client.GetAsync<List<DiscordServer>>(request);

            var adminServers = response.Where(s => s.HasAdmin())
                .ToList();
            return View(adminServers);
            
            //var guilds = JsonDocument.Parse(response.Content);

            //var request = new HttpRequestMessage(HttpMethod.Get, discordConfig.ApiRoot + "/users/@me/guilds");
            //request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            //request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            //
            //var response = await context.Backchannel.SendAsync(
            //    request,
            //    HttpCompletionOption.ResponseHeadersRead,
            //    context.HttpContext.RequestAborted);
            //response.EnsureSuccessStatusCode();
            //
            //var guilds = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        }
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}