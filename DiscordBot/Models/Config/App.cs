namespace DiscordBot.Models.Config;

public class App
{
    public const string Section = "App";
    public bool RunInit { get; set; } = true;
    public bool RunScheduler { get; set; } = true;
}