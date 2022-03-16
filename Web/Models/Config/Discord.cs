namespace Web.Models.Config;

public class Discord
{
    public const string Section = "Discord";
    public string Token { get; set; }
    public string ApiRoot { get; set; }
    public string ClientId { get; set; }
    public string ClientSecret { get; set; }
}