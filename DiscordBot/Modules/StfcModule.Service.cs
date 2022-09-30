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

// FILE IS IN PRE-DRAFT STATE
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Interactions;
using Discord.Net;
using DiscordBot.AutocompleteHandlers;
using DiscordBot.Domain.Entities.Services;
using DiscordBot.Domain.Entities.Zones;
using Microsoft.Extensions.DependencyInjection;

namespace DiscordBot.Modules
{
    public partial class StfcModule
    {
        [SlashCommand("service-show", "Shows all the services available in the zone")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task ShowServices(
            [Summary("Zone", "Zone Name")][Autocomplete(typeof(ZoneNames))] string name)
        {
            using var serviceScope = _serviceProvider.CreateScope();
            var zoneRepository = serviceScope.ServiceProvider.GetService<IZoneRepository>();
            var serviceRepository = serviceScope.ServiceProvider.GetService<IServiceRepository>();

            var thisZone = await zoneRepository.GetByNameAsync(name);
            if (thisZone == null)
            {
                await this.RespondAsync(
                    "The zone could not be found.",
                    ephemeral: true);
            }
            else
            {
                var builder = new ComponentBuilder();
                var services = await serviceRepository.GetByZoneIdAsync(thisZone.Id);
                if (!services.Any())
                {
                    await this.RespondAsync(
                        "No services have been defined for this zone.",
                        ephemeral: true);
                }
                else
                {
                    var serviceMenu = new SelectMenuBuilder()
                    {
                        CustomId = "service-select",
                        Placeholder = "Select a service",
                        MinValues = 1,
                        MaxValues = 1
                    };

                    foreach (var service in services.OrderBy(s => s.Name))
                    {
                        serviceMenu.AddOption(service.Name, service.Id.ToString(), service.Description);
                    }
                    builder.WithSelectMenu(serviceMenu);
                }

                var addServiceButton = new ButtonBuilder(
                    "Add Service",
                    "service-add-" + thisZone.Id);
                builder.WithButton(addServiceButton);

                await this.RespondAsync(
                    "Services for " + thisZone.Name + ":",
                    components: builder.Build(),
                    ephemeral: true);
            }
        }
    }
}