using System;
namespace DiscordBot.AdminWeb.Models.Config
{
    public class Discord
    {
        public const string SECTION = "Discord";
        public string Token { get; set; }
        public string Prefix { get; set; }
        public string WatchingStatus { get; set; }
        public int SchedulePollSeconds { get; set; }
    }
}
