namespace DiscordBot.Models.Config;

public class Discord
{
    public const string Section = "Discord";
    public string Token { get; set; }
    public string Prefix { get; set; }
    public string WatchingStatus { get; set; }
    public int SchedulePollSeconds { get; set; }
}