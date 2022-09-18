using DiscordBot.Domain.Entities.Alliances;
using DiscordBot.Domain.Seedwork;

namespace DiscordBot.Domain.Entities.Services
{
    public class AllianceService : Entity
    {
        public AllianceService()
        {
            
        }
        public AllianceService(Alliance alliance, Service service, AllianceServiceLevel allianceServiceLevel)
        {
            Alliance = alliance;
            _allianceId = alliance.Id;
            Service = service;
            _serviceId = service.Id;
            
            if (allianceServiceLevel == null) allianceServiceLevel = AllianceServiceLevel.Undefined;
            AllianceServiceLevel = allianceServiceLevel;
            _allianceServiceLevelId = allianceServiceLevel.Id;
        }
        
        public virtual Alliance Alliance { get; private set; }
        private long? _allianceId;
        
        public virtual Service Service { get; private set; }
        private long? _serviceId;
        
        public virtual AllianceServiceLevel AllianceServiceLevel { get; private set; }
        private int? _allianceServiceLevelId;
    }
}