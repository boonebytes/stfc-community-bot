namespace Web.Helpers;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;


public static class SessionKeys
{
    public const string FriendlyName = "FriendlyName"; 
    public const string AccessToken = "AccessToken";
    public const string RefreshToken = "RefreshToken";
}

public static class SessionHelper
{
    public static string GetUsername(this ISession session)
    {
        return session.GetString(SessionKeys.FriendlyName) ?? string.Empty;

        return session.GetString(SessionKeys.FriendlyName);
        //return  User.Claims.First(c => c.Type == ClaimTypes.Name && !System.Text.RegularExpressions.Regex.IsMatch(c.Value, "^[0-9]*$"))
    }

    public static void SetUsername(this ISession session, string value)
    {
        session.SetString(SessionKeys.FriendlyName, value);
    }
}