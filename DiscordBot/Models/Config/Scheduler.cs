namespace DiscordBot.Models.Config;

public class Scheduler
{
    public const string Section = "Scheduler";
    
    public string RamName { get; set; } = "RamJobScheduler";
    public int RamMaxConcurrency { get; set; } = 5;
    
    public string PersistentName { get; set; } = "PersistentJobScheduler";
    public int PersistentMaxConcurrency { get; set; } = 5;
    public string PersistentDbConnection { get; set; } = "";
}