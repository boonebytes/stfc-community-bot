using System;
using DiscordBot.Domain.Entities.Alliances;

namespace DiscordBot.AdminWeb.ViewModels
{
    public class ZoneViewModel
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public int? Level { get; set; }
        public string Threats { get; set; }
        public string DefendUtcDayOfWeek { get; set; }
        public string DefendUtcTime { get; set; }
        public string Notes { get; set; }

        public long? OwnerId { get; set; }
        public Alliance Owner { get; set; }
        public DateTime? NextDefend { get; set; }
    }
}
