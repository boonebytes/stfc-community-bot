using System;
namespace DiscordBot.Models.Config
{
    public class Discord
    {
        public const string SECTION = "Discord";
        public string Token { get; set; }
        public string Prefix { get; set; }
    }
}
