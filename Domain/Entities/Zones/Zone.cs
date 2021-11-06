using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using DiscordBot.Domain.Entities.Alliances;
using DiscordBot.Domain.Events;
using DiscordBot.Domain.Seedwork;
using DiscordBot.Domain.Shared;

namespace DiscordBot.Domain.Entities.Zones
{
    public partial class Zone : Entity
    {
        public Zone()
        {
            _starSystems = new List<StarSystem>();
        }

        public virtual string Name { get; private set; }
        public virtual int Level { get; private set; }
        public virtual string Threats { get; private set; }
        public virtual string DefendUtcDayOfWeek { get; private set; }
        public virtual string DefendUtcTime { get; private set; }
        public virtual DayOfWeek? DefendEasternDay { get; private set; }
        public virtual TimeSpan? DefendEasternTime { get; private set; }
        public virtual string Notes { get; private set; }

        private long? _ownerId;
        public virtual Alliance Owner { get; private set; }

        private readonly List<StarSystem> _starSystems;
        public IReadOnlyCollection<StarSystem> StarSystems => _starSystems;

        private readonly List<ZoneNeighbour> _neighbours;
        public IReadOnlyCollection<Zone> ZoneNeighbours => (IReadOnlyCollection<Zone>) _neighbours.Select(z => z.ToZone);

        public bool LowRisk
        {
            get
            {
                if (Level > 1 && string.IsNullOrEmpty(Threats))
                    return true;
                else
                    return false;
            }
        }

        public virtual DateTime? NextDefend { get; private set; }

        public void AddNeighbour(Zone newZone)
        {
            _neighbours.Add(new ZoneNeighbour(this, newZone));
        }

        public void RemoveNeighbour(Zone zone)
        {
            var found = _neighbours.Where(z => z.ToZone == zone);
            if (found.Count() == 1)
            {
                _neighbours.Remove(found.First());
            }
            else
            {
                throw new NullReferenceException($"Unable to find child neighbour {zone.Id}");
            }
        }

        public DateTime GetNextWeekDefend()
        {
            if (!NextDefend.HasValue) SetNextDefend();

            var response = NextDefend.Value;
            if (DefendEasternDay >= DateTime.Now.ToEasternTime().DayOfWeek)
            {
                response = response.AddDays(7);
            }
            return response;
        }

        public void SetNextDefend(bool forceUpdate = false)
        {
            if (!forceUpdate && NextDefend.HasValue && NextDefend > DateTime.UtcNow)
            {
                return;
            }

            CultureInfo culture = new CultureInfo("en-US");
            DayOfWeek dayOfWeek;
            switch (DefendUtcDayOfWeek.ToLower())
            {
                case "sunday":
                    dayOfWeek = DayOfWeek.Sunday;
                    break;
                case "monday":
                    dayOfWeek = DayOfWeek.Monday;
                    break;
                case "tuesday":
                    dayOfWeek = DayOfWeek.Tuesday;
                    break;
                case "wednesday":
                    dayOfWeek = DayOfWeek.Wednesday;
                    break;
                case "thursday":
                    dayOfWeek = DayOfWeek.Thursday;
                    break;
                case "friday":
                    dayOfWeek = DayOfWeek.Friday;
                    break;
                case "saturday":
                    dayOfWeek = DayOfWeek.Saturday;
                    break;
                default:
                    throw new InvalidCastException("Cannot cast day of week " + DefendUtcDayOfWeek);
            }

            DateTime result = DateTime.ParseExact(DefendUtcTime, "h:mm tt", culture, DateTimeStyles.AssumeUniversal).ToUniversalTime();
            if (dayOfWeek != DateTime.UtcNow.DayOfWeek)
            {
                result = result.AddDays((-1 * (int)DateTime.UtcNow.DayOfWeek) + ((int)dayOfWeek));

            }
            if (result < DateTime.UtcNow) result = result.AddDays(7);
            NextDefend = result.ToUniversalTime();

            var easternTime = NextDefend.Value.ToEasternTime();
            DefendEasternDay = easternTime.DayOfWeek;
            DefendEasternTime = easternTime.TimeOfDay;
        }

        public string GetDiscordEmbedName()
        {
            if (_ownerId.HasValue)
                return $"{Owner.Acronym} - {Name} ({Level}^)";
            else
                return $"Unclaimed - {Name} ({Level}^)";
        }

        public string GetDiscordEmbedValue(bool shortVersion = false, bool useNextWeek = false)
        {
            string response = "";
            //var tz = TimeZoneInfo.ConvertTime(NextDefend.Value, )

            if (shortVersion)
            {
                response = $"{Owner.Acronym}/{Name}({Level}^): <t:";
                if (useNextWeek)
                {
                    response += NextDefend.Value.ToUniversalTime().AddDays(7).Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
                }
                else
                {
                    response += NextDefend.Value.ToUniversalTime().Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
                }
                response += $":t> local / {NextDefend.Value.ToEasternTime().ToString("h:mm tt")} ET";
                if (!string.IsNullOrEmpty(Threats))
                    response += " [*_" + Threats + "_*]";
                else if (LowRisk)
                    response += " [*_Low Risk_*]";
            }
            else
            {
                if (LowRisk)
                    response += "*_Low Risk_*\n";

                response += $"**When**: "
                            + $"<t:"; //
                if (useNextWeek)
                {
                    response += NextDefend.Value.ToUniversalTime().AddDays(7).Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
                }
                else
                {
                    response += NextDefend.Value.ToUniversalTime().Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
                }
                response += ":t> local / "
                            + $"{DefendUtcTime} UTC / "
                            + $"{NextDefend.Value.ToEasternTime().ToString("h:mm tt")} ET";
                response += "\n**Threats**: " + (string.IsNullOrEmpty(Threats) ? "None" : Threats);
                if (!string.IsNullOrEmpty(Notes))
                {
                    response += $"\n**Notes**: {Notes}";
                }
            }

            return response;
        }

        public void SetName(string name)
        {
            Name = name;
            this.AddZoneChangedDomainEvent();
        }

        public void SetLevel(int level)
        {
            if (level < 1 || level > 3)
                throw new ArgumentOutOfRangeException("Level must be between 1 and 3");

            Level = level;
            this.AddZoneChangedDomainEvent();
        }

        public void SetOwnerId(long id)
        {
            _ownerId = id;
            this.AddZoneChangedDomainEvent();
        }

        public void AddStarSystem(StarSystem starSystem)
        {
            if (!_starSystems.Contains(starSystem))
                _starSystems.Add(starSystem);
        }

        public void AddZoneChangedDomainEvent()
        {
            this.AddDomainEvent(new ZoneUpdatedDomainEvent(this));
        }

    }
}
