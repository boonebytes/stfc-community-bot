using System.Diagnostics;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Web.Models;

namespace Web.Controllers;

public abstract class BaseController : Controller
{
    protected readonly ILogger<BaseController> Logger;
    protected readonly SignInManager<AppUser> SignInManager;

    public BaseController(ILogger<BaseController> logger, SignInManager<AppUser> signInManager)
    {
        Logger = logger;
        SignInManager = signInManager;
    }

    public override void OnActionExecuting(ActionExecutingContext context)
    {
        base.OnActionExecuting(context);
        VerifySession();
    }

    private void VerifySession()
    {
        if (SignInManager.IsSignedIn(User))
        {
            if (HttpContext.Session.GetUsername() == "")
            {
                var username = User.Claims.First(c =>
                        c.Type == ClaimTypes.Name && !System.Text.RegularExpressions.Regex.IsMatch(c.Value, "^[0-9]*$"))
                    .Value;
                HttpContext.Session.SetUsername(username);
            }
        }
    }
}