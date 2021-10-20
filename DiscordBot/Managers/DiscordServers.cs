using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.Extensions.Logging;

namespace DiscordBot.Managers
{
    public class DiscordServers
    {
        protected List<Models.Tdl.DiscordServer> _discordServers = new List<Models.Tdl.DiscordServer>();

        private readonly ILogger<DiscordServers> _logger;

        public DiscordServers(ILogger<DiscordServers> logger)
        {
            _logger = logger;

            try
            {
                using (TextReader reader = new StreamReader("DiscordServers.csv", Encoding.Default))
                {
                    using (var csvReader = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)))
                    {
                        _discordServers = csvReader.GetRecords<Models.Tdl.DiscordServer>().ToList();
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unable to load CSV file");
                throw;
            }
        }

        public List<Models.Tdl.DiscordServer> Servers
        {
            get
            {
                return _discordServers;
            }
        }
    }
}
