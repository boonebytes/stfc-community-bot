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
using DiscordBot.Services;
using Quartz;

namespace DiscordBot.Jobs;

public class MonitorScheduler : BaseJob
{
    //private readonly ILogger<MonitorScheduler> _logger;
    private readonly IAllianceRepository _allianceRepository;
    private readonly IZoneRepository _zoneRepository;
    
    public MonitorScheduler(ILogger<MonitorScheduler> logger, IZoneRepository zoneRepository, IAllianceRepository allianceRepository) : base(logger)
    {
        //_logger = logger;
        _zoneRepository = zoneRepository;
        _allianceRepository = allianceRepository;
    }

    protected override async Task DoWork(IJobExecutionContext context)
    {
        Logger.LogInformation("Cleaning up interaction response cache");
        _ = InteractionHandler.CleanRespondingInteractionCache();
        Logger.LogInformation("Running the MonitorScheduler job");
        await _allianceRepository.InitPostSchedule();
        await _zoneRepository.InitZones();
    }
}