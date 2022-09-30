/*
Copyright 2022 Boonebytes

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/

using DiscordBot.Domain.Entities.Alliances;
using DiscordBot.Domain.Entities.Zones;
using Quartz;

namespace DiscordBot;

public partial class Scheduler
{
    private readonly ILogger<Scheduler> _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly Models.Config.Scheduler _schedulerConfig;
    private readonly ISchedulerFactory _schedulerFactory;
    private IScheduler _ramScheduler;
    private IScheduler _persistentScheduler;

    public Scheduler(
        ILogger<Scheduler> logger,
        IServiceProvider serviceProvider, Models.Config.Scheduler schedulerConfig, ISchedulerFactory schedulerFactory)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
        _schedulerConfig = schedulerConfig;
        _schedulerFactory = schedulerFactory;
    }

    public async Task HandleZoneUpdatedAsync(long zoneId)
    {
        _logger.LogInformation("Zone Update received: {ZoneId}", zoneId);
        
        using var thisServiceScope = _serviceProvider.CreateScope();
        var zoneRepository = thisServiceScope.ServiceProvider.GetService<IZoneRepository>();
        var zone = await zoneRepository.GetAsync(zoneId);
        await AddOrUpdateZoneDefend(thisServiceScope, zone);

        if (zone.Owner != null)
        {
            var allianceRepository = thisServiceScope.ServiceProvider.GetService<IAllianceRepository>();
            var allies = allianceRepository.GetTerritoryHelpersFromOwnerAlliance(zone.Owner.Id);
            
            foreach (var ally in allies)
            {
                await AddOrUpdateZoneAssist(thisServiceScope, zone, ally);
            }
        }
    }
}