using System.Security.Claims;
using Discord;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Web.Models;

namespace Web.Controllers;

public class AccountController: BaseController
{
    private readonly UserManager<AppUser> _userManager;

    private const string LoginProviderKey = "Discord";

    [TempData] public string ErrorMessage { get; set; } = "";

    public AccountController(ILogger<AccountController> logger, SignInManager<AppUser> signInManager, UserManager<AppUser> userManager) : base(logger, signInManager)
    {
        _userManager = userManager;
    }
    
    // GET
    public IActionResult Index()
    {
        return View();
    }

    [AllowAnonymous]
    public async Task<IActionResult> Login()
    {
        if (!string.IsNullOrEmpty(ErrorMessage))
        {
            ModelState.AddModelError(string.Empty, ErrorMessage);
        }
        return View();
    }

    [AllowAnonymous]
    public async Task<IActionResult> StartLogin()
    {
        await HttpContext.SignOutAsync();

        var redirectUrl = Url.Action("CompleteLogin");
        var properties = SignInManager.ConfigureExternalAuthenticationProperties(LoginProviderKey, redirectUrl);
        return new ChallengeResult(LoginProviderKey, properties);
    }
    
    [AllowAnonymous]
    public async Task<IActionResult> CompleteLogin(string remoteError = "")
    {
        if (remoteError != "")
        {
            ErrorMessage = $"Error from external provider: {remoteError}";
            return RedirectToAction("Login");
        }
        
        var info = await SignInManager.GetExternalLoginInfoAsync();

        if (info == null)
        {
            ErrorMessage = "Error loading external login information.";
            return RedirectToAction("Login");
        }
        
        // Sign in the user with this external login provider if the user already 
        // has a login.
        var signInResult = await SignInManager.ExternalLoginSignInAsync(
            info.LoginProvider, 
            info.ProviderKey,
            isPersistent: false,
            bypassTwoFactor : true);
        
        
        if (signInResult.Succeeded)
        {
            // Store the access token and resign in so the token is included in
            // in the cookie
            var user = await _userManager.FindByLoginAsync(info.LoginProvider, info.ProviderKey);

            var props = new AuthenticationProperties();
            props.StoreTokens(info.AuthenticationTokens);

            await SignInManager.SignInAsync(user, props, info.LoginProvider);

            HttpContext.Session.SetString(SessionKeys.AccessToken,
                info.AuthenticationTokens.Single(t => t.Name == "access_token").Value);
            HttpContext.Session.SetString(SessionKeys.RefreshToken,
                info.AuthenticationTokens.Single(t => t.Name == "refresh_token").Value);

            Logger.LogInformation("{Name} logged in with {LoginProvider} provider.", 
                info.Principal.Identity?.Name, info.LoginProvider);

            return RedirectToAction("Index", "Home");
        }
        
        if (signInResult.IsLockedOut)
        {
            return RedirectToAction("Lockout");
        }
        else
        {
            // If the user does not have an account, then ask the user to create an 
            // account.
            //LoginProvider = info.LoginProvider;
            //
            //if (info.Principal.HasClaim(c => c.Type == ClaimTypes.Email))
            //{
            //    Input = new InputModel
            //    {
            //        Email = info.Principal.FindFirstValue(ClaimTypes.Email)
            //    };
            //}
            //
            //return Page();

            var newUser = new AppUser
            {
                UserName = info.ProviderKey
            };

            var newUserResult = await _userManager.CreateAsync(newUser);
            if (newUserResult.Succeeded)
            {
                newUserResult = await _userManager.AddLoginAsync(newUser, info);

                if (newUserResult.Succeeded)
                {
                    // If they exist, add claims to the user
                    if (info.Principal.HasClaim(c => c.Type == ClaimTypes.NameIdentifier))
                    {
                        await _userManager.AddClaimAsync(newUser,
                            info.Principal.FindFirst(ClaimTypes.NameIdentifier));
                    }

                    if (info.Principal.HasClaim(c => c.Type == ClaimTypes.Name))
                    {
                        await _userManager.AddClaimAsync(newUser,
                            info.Principal.FindFirst(ClaimTypes.Name));
                    }

                    // Include the access token in the properties
                    var props = new AuthenticationProperties();
                    props.StoreTokens(info.AuthenticationTokens);
                    props.IsPersistent = true;

                    await SignInManager.SignInAsync(newUser, props);

                    Logger.LogInformation(
                        "User created an account using {Name} provider",
                        info.LoginProvider);

                    return RedirectToAction("Index", "Home");
                }
            }
            foreach (var error in newUserResult.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View("Login");
        }
    }

    [AllowAnonymous]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync();
        HttpContext.Session.Clear();
        return RedirectToAction("Login");
    }

    public IActionResult Lockout()
    {
        return View();
    }
}