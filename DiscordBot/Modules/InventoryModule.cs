/*
Copyright 2023 Boonebytes

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

using System.Globalization;
using System.IO;
using System.Text;
using System.Text.Unicode;
using Discord;
using Discord.Interactions;
using DiscordBot.Domain.Entities.Alliances;
using DiscordBot.Domain.Entities.Request;
using DiscordBot.Domain.Entities.Services;
using DiscordBot.Domain.Entities.Zones;
using DiscordBot.Domain.Shared;
using DiscordBot.Html;

namespace DiscordBot.Modules;

[DefaultMemberPermissions(GuildPermission.ManageGuild)]
[Group("inventory", "Show Alliance Inventory")]
public class InventoryModule : BaseModule
{

    public InventoryModule(ILogger<InventoryModule> logger, IServiceProvider serviceProvider) : base(logger, serviceProvider)
    {
    }
    
    [SlashCommand("update", "Update alliance inventory")]
    public async Task UpdateAsync(
        [Summary("EffectiveDate", "EffectiveDate (yyyy-mm-dd)")] string effectiveDate,
        [Summary("Isogen1", "Refined Isogen 1")] ulong isogen1,
        [Summary("Isogen2", "Refined Isogen 2")] ulong isogen2,
        [Summary("Isogen3", "Refined Isogen 3")] ulong isogen3,
        [Summary("Cores", "Progenitor Cores")] ulong cores,
        [Summary("Diodes", "Progenitor Diodes")] ulong diodes,
        [Summary("Emitters", "Progenitor Emitters")] ulong emitters,
        [Summary("Reactors", "Progenitor Reactors")] ulong reactors,
        [Summary("Reserves", "Alliance Reserves")] ulong reserves,
        [Summary("CollisionalPlasma", "Collisional Plasma")] ulong collisionalPlasma,
        [Summary("MagneticPlasma", "MagneticPlasma")] ulong magneticPlasma,
        [Summary("Superconductors", "Superconductors")] ulong superconductors
    )
    {
        if (Context.Channel is IPrivateChannel channel)
        {
            await RespondAsync("That command isn't valid in DMs.", ephemeral: true);
            return;
        }
        
        try
        {
            using var serviceScope = ServiceProvider.CreateScope();
            var allianceRepository = serviceScope.ServiceProvider.GetService<IAllianceRepository>();
            var thisAlliance = allianceRepository.FindFromGuildId(Context.Guild.Id);

            if (thisAlliance == null)
            {
                await RespondAsync("Unable to determine alliance from this channel", ephemeral: true);
                return;
            }

            await DeferAsync(ephemeral: true);
            
            serviceScope.ServiceProvider.GetService<RequestContext>().Init(thisAlliance.Id);

            DateTime actualEffectiveDate = DateTime.MinValue;
            var dateParsed = DateTime.TryParseExact(
                effectiveDate,
                "yyyy-MM-dd",
                CultureInfo.InvariantCulture,
                System.Globalization.DateTimeStyles.AssumeUniversal,
                out actualEffectiveDate);
            if (!dateParsed || actualEffectiveDate == DateTime.MinValue)
            {
                await ModifyResponseAsync("Unable to parse date. Please try again.", true);
                return;
            }

            await allianceRepository.UpdateInventory(
                actualEffectiveDate, isogen1, isogen2, isogen3, cores, diodes, emitters, reactors,
                reserves, collisionalPlasma, magneticPlasma, superconductors);

            var result = "Done! Saved values:\n"
                         + "Effective Date: " + actualEffectiveDate.ToString("yyyy-MM-dd") + "\n"
                         + "Isogen 1: " + isogen1.ToString("#,##0") + "\n"
                         + "Isogen 2: " + isogen2.ToString("#,##0") + "\n"
                         + "Isogen 3: " + isogen3.ToString("#,##0") + "\n"
                         + "Cores: " + cores.ToString("#,##0") + "\n"
                         + "Diodes: " + diodes.ToString("#,##0") + "\n"
                         + "Emitters: " + emitters.ToString("#,##0") + "\n"
                         + "Reactors: " + reactors.ToString("#,##0") + "\n"
                         + "Reserves: " + reserves.ToString("#,##0") + "\n"
                         + "Collisional Plasma: " + collisionalPlasma.ToString("#,##0") + "\n"
                         + "Magnetic Plasma: " + magneticPlasma.ToString("#,##0") + "\n"
                         + "Superconductors: " + superconductors.ToString("#,##0");
            
            await ModifyResponseAsync(result, true);
        }
        catch (Exception e)
        {
            Logger.LogError(e, "Error in UpdateInventoryAsync");
        }
    }

    [SlashCommand("show-territory", "Show alliance territory inventory")]
    public async Task ShowTerritoryAsync()
    {
        if (Context.Channel is IPrivateChannel channel)
        {
            await RespondAsync("That command isn't valid in DMs.", ephemeral: true);
            return;
        }
        
        try
        {
            using var serviceScope = ServiceProvider.CreateScope();
            var allianceRepository = serviceScope.ServiceProvider.GetService<IAllianceRepository>();
            var thisAlliance = allianceRepository.FindFromGuildId(Context.Guild.Id);

            if (thisAlliance == null)
            {
                await RespondAsync("Unable to determine alliance from this channel", ephemeral: true);
                return;
            }

            await DeferAsync(ephemeral: true);
            
            serviceScope.ServiceProvider.GetService<RequestContext>().Init(thisAlliance.Id);

            var territoryInventory = await allianceRepository.GetTerritoryInventory(thisAlliance);
            var fileContents = await Generator.GetTerritoryInventory(thisAlliance, territoryInventory);
            var image = await Generator.ConvertToImage(fileContents);
            using var fileStream = new MemoryStream();
            //fileStream.Write(Encoding.UTF8.GetBytes(fileContents));
            fileStream.Write(image);
            fileStream.Position = 0; 
            var attachment = new FileAttachment(fileStream, "TerritoryInventory.png", "Territory Inventory Balances");
            await Context.Channel.SendFileAsync(attachment);
            
            await ModifyResponseAsync("Done!", true);
        }
        catch (Exception e)
        {
            Logger.LogError(e, "Error in TerritoryInventoryShowAsync");
        }
    }
    
    [SlashCommand("show-starbase", "Show alliance starbase inventory")]
    public async Task ShowStarbaseAsync()
    {
        if (Context.Channel is IPrivateChannel channel)
        {
            await RespondAsync("That command isn't valid in DMs.", ephemeral: true);
            return;
        }
        
        try
        {
            using var serviceScope = ServiceProvider.CreateScope();
            var allianceRepository = serviceScope.ServiceProvider.GetService<IAllianceRepository>();
            var thisAlliance = allianceRepository.FindFromGuildId(Context.Guild.Id);

            if (thisAlliance == null)
            {
                await RespondAsync("Unable to determine alliance from this channel", ephemeral: true);
                return;
            }

            await DeferAsync(ephemeral: true);
            
            serviceScope.ServiceProvider.GetService<RequestContext>().Init(thisAlliance.Id);

            var starbaseInventory = await allianceRepository.GetStarbaseInventory(thisAlliance);
            var fileContents = await Generator.GetStarbaseInventory(thisAlliance, starbaseInventory);
            var image = await Generator.ConvertToImage(fileContents);
            using var fileStream = new MemoryStream();
            //fileStream.Write(Encoding.UTF8.GetBytes(fileContents));
            fileStream.Write(image);
            fileStream.Position = 0; 
            var attachment = new FileAttachment(fileStream, "StarbaseInventory.png", "Starbase Inventory Balances");
            await Context.Channel.SendFileAsync(attachment);
            
            await ModifyResponseAsync("Done!", true);
        }
        catch (Exception e)
        {
            Logger.LogError(e, "Error in StarbaseInventoryShowAsync");
        }
    }
}