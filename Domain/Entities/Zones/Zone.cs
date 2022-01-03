using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using DiscordBot.Domain.Entities.Alliances;
using DiscordBot.Domain.Entities.Services;
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
            _services = new List<Service>();
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

        private readonly List<ZoneNeighbour> _zoneNeighbours;
        public IReadOnlyCollection<ZoneNeighbour> ZoneNeighbours => _zoneNeighbours;

        //private readonly List<ZoneNeighbour> _zoneNeighboursIn;
        //public IReadOnlyCollection<ZoneNeighbour> ZoneNeighboursIn => _zoneNeighboursIn;

        public IReadOnlyCollection<Zone> Neighbours => _zoneNeighbours.Select(zn => zn.ToZone).ToList();

        private readonly List<Service> _services;
        public IReadOnlyCollection<Service> Services => _services;

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
            _zoneNeighbours.Add(new ZoneNeighbour(this, newZone));
        }

        public void RemoveNeighbour(Zone zone)
        {
            var found = _zoneNeighbours.Where(z => z.ToZone == zone);
            if (found.Count() == 1)
            {
                _zoneNeighbours.Remove(found.First());
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

        public void SetThreats(string threats)
        {
            Threats = threats;
        }

        public void AddStarSystem(StarSystem starSystem)
        {
            if (!_starSystems.Contains(starSystem))
                _starSystems.Add(starSystem);
        }

        public void AddService(Service service)
        {
            if (!_services.Contains(service))
                _services.Add(service);
        }

        public void RemoveService(Service service)
        {
            if (_services.Contains(service))
                _services.Remove(service);
        }

        public void AddZoneChangedDomainEvent()
        {
            this.AddDomainEvent(new ZoneUpdatedDomainEvent(this));
        }

    }
}
