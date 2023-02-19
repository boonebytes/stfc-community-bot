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
using Humanizer;

namespace DiscordBot.Modules;

[DefaultMemberPermissions(GuildPermission.ManageGuild)]
[Group("inventory", "Show Alliance Inventory")]
public class InventoryModule : BaseModule
{

    public InventoryModule(ILogger<InventoryModule> logger, IServiceProvider serviceProvider) : base(logger, serviceProvider)
    {
    }

    private decimal parseMetricToDecimal(string input)
    {
        return (decimal) input.ToLower().Replace("m", "M").FromMetric();
    }
    
    [SlashCommand("update", "Update alliance inventory")]
    public async Task UpdateAsync(
        [Summary("EffectiveDate", "EffectiveDate (yyyy-mm-dd)")] string effectiveDate,
        [Summary("Isogen1", "Refined Isogen 1")] string isogen1,
        [Summary("Isogen2", "Refined Isogen 2")] string isogen2,
        [Summary("Isogen3", "Refined Isogen 3")] string isogen3,
        [Summary("Cores", "Progenitor Cores")] string cores,
        [Summary("Diodes", "Progenitor Diodes")] string diodes,
        [Summary("Emitters", "Progenitor Emitters")] string emitters,
        [Summary("Reactors", "Progenitor Reactors")] string reactors,
        [Summary("Reserves", "Alliance Reserves")] string reserves,
        [Summary("CollisionalPlasma", "Collisional Plasma")] string collisionalPlasma,
        [Summary("MagneticPlasma", "MagneticPlasma")] string magneticPlasma,
        [Summary("Superconductors", "Superconductors")] string superconductors
    )
    {
        var deferred = false;
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
            deferred = true;
            
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
                actualEffectiveDate,
                parseMetricToDecimal(isogen1),
                parseMetricToDecimal(isogen2),
                parseMetricToDecimal(isogen3),
                parseMetricToDecimal(cores),
                parseMetricToDecimal(diodes),
                parseMetricToDecimal(emitters),
                parseMetricToDecimal(reactors),
                parseMetricToDecimal(reserves),
                parseMetricToDecimal(collisionalPlasma),
                parseMetricToDecimal(magneticPlasma),
                parseMetricToDecimal(superconductors)
                );

            var result = "Done! Saved values:\n"
                         + "Effective Date: " + actualEffectiveDate.ToString("yyyy-MM-dd") + "\n"
                         + "Isogen 1: " + parseMetricToDecimal(isogen1).ToString("#,##0") + "\n"
                         + "Isogen 2: " + parseMetricToDecimal(isogen2).ToString("#,##0") + "\n"
                         + "Isogen 3: " + parseMetricToDecimal(isogen3).ToString("#,##0") + "\n"
                         + "Cores: " + parseMetricToDecimal(cores).ToString("#,##0") + "\n"
                         + "Diodes: " + parseMetricToDecimal(diodes).ToString("#,##0") + "\n"
                         + "Emitters: " + parseMetricToDecimal(emitters).ToString("#,##0") + "\n"
                         + "Reactors: " + parseMetricToDecimal(reactors).ToString("#,##0") + "\n"
                         + "Reserves: " + parseMetricToDecimal(reserves).ToString("#,##0") + "\n"
                         + "Collisional Plasma: " + parseMetricToDecimal(collisionalPlasma).ToString("#,##0") + "\n"
                         + "Magnetic Plasma: " + parseMetricToDecimal(magneticPlasma).ToString("#,##0") + "\n"
                         + "Superconductors: " + parseMetricToDecimal(superconductors).ToString("#,##0");
            
            await ModifyResponseAsync(result, true);
        }
        catch (Exception e)
        {
            Logger.LogError(e, "Error in UpdateInventoryAsync");
            try
            {
                if (deferred)
                {
                    await ModifyResponseAsync("An unexpected error has occurred. Please try your request again.", true);
                }
            }
            catch (Exception ex)
            {
                // ignored
            }
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
