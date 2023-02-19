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

using System.IO;
using DiscordBot.Domain.Entities.Alliances;
using DiscordBot.Domain.Summaries;
using HandlebarsDotNet;
using PuppeteerSharp;

namespace DiscordBot.Html;

public class Generator
{
    private static bool _initComplete = false;
    private static SemaphoreSlim _fetchLock = new(1, 1);
    private static HandlebarsTemplate<object, object> _territoryInventoryTemplate = null;
    private static HandlebarsTemplate<object, object> _starbaseInventoryTemplate = null;

    private static async Task Init()
    {
        if (!_initComplete)
        {
            /*
            Handlebars.RegisterHelper("formatDate", (output, options, context, arguments) =>
            {
                var dt = Convert.ToDateTime(arguments[0]);
                output.Write(dt.ToString("d-MMM-yyyy"));
            });
            */
        
            var dateFormatter = new CustomDateTimeFormatter("MMM d, yyyy");
            Handlebars.Configuration.FormatterProviders.Add(dateFormatter);
            
            var negativeDecimalFormatter = new NegativeDecimalFormatter("#,##0");
            Handlebars.Configuration.FormatterProviders.Add(negativeDecimalFormatter);

            var partialHeader = await GetTemplate("Html/_header.html");
            Handlebars.RegisterTemplate("header", partialHeader);
            _initComplete = true;
        }
    }
    
    private static async Task<string> GetTemplate(string filename)
    {
        var files = typeof(Generator).Assembly.GetManifestResourceNames();
        
        var fileRef = "DiscordBot." + filename.Replace("/", ".");
        //var manifestEmbeddedProvider = new ManifestEmbeddedFileProvider(typeof(Generator).Assembly);
        //var file = manifestEmbeddedProvider.GetFileInfo(fileRef);
        //await using var stream = file.CreateReadStream(); 
        await using var stream = typeof(Generator).Assembly.GetManifestResourceStream(fileRef);
        using var reader = new StreamReader(stream);
        var content = await reader.ReadToEndAsync();
        return content;
    }

    public static async Task<byte[]> ConvertToImage(string html, int width = 800)
    {
#if DEBUG
        await _fetchLock.WaitAsync();
        try
        {
            using var browserFetcher = new BrowserFetcher();
            await browserFetcher.DownloadAsync(BrowserFetcher.DefaultChromiumRevision);
        }
        finally
        {
            _fetchLock.Release();
        }

        var browser = await Puppeteer.LaunchAsync(new LaunchOptions
        {
            Headless = true
        });
#else
        // This assumes that all release builds are executed in Docker.
        // Note: The "no-sandbox" is not any more-safe in Docker; only
        // use that argument if you trust your HTML source!
        
        var browser = await Puppeteer.LaunchAsync(new LaunchOptions
        {
            Headless = true,
            Args = new[] {"--no-sandbox"}
        });
#endif        
        
        var page = await browser.NewPageAsync();
        await page.SetViewportAsync(new ViewPortOptions
        {
            Width = width,
            Height = 1
        });
        await page.SetContentAsync(html);
        var screenshot = await page.ScreenshotDataAsync(new ScreenshotOptions
        {
            FullPage = true
        });
        return screenshot;
    }

    public static async Task<string> GetTerritoryInventory(Alliance alliance, List<TerritoryInventory> inventory)
    {
        await Init();
        if (_territoryInventoryTemplate is null)
        {
            var templateSource = await GetTemplate("Html/Inventory/Territory.html");
            _territoryInventoryTemplate = Handlebars.Compile(templateSource);
        }
        
        var data = new
        {
            Alliance = alliance.Acronym,
            Rows = inventory
        };
        var results = _territoryInventoryTemplate(data);
        return results;
    }
    
    public static async Task<string> GetStarbaseInventory(Alliance alliance, List<StarbaseInventory> inventory)
    {
        await Init();
        if (_starbaseInventoryTemplate is null)
        {
            var templateSource = await GetTemplate("Html/Inventory/Starbase.html");
            _starbaseInventoryTemplate = Handlebars.Compile(templateSource);
        }
        
        var data = new
        {
            Alliance = alliance.Acronym,
            Rows = inventory
        };
        var results = _starbaseInventoryTemplate(data);
        return results;
    }
}