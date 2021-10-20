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
    public class DefendTimes
    {
        protected List<Models.Tdl.Zone> _zones = new List<Models.Tdl.Zone>();

        private readonly ILogger<DefendTimes> _logger;

        public DefendTimes(ILogger<DefendTimes> logger)
        {
            _logger = logger;

            try
            {
                using (TextReader reader = new StreamReader("Defends.csv", Encoding.Default))
                {
                    using (var csvReader = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)))
                    {
                        _zones = csvReader.GetRecords<Models.Tdl.Zone>().ToList();
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unable to load CSV file");
                throw;
            }
        }

        public List<Models.Tdl.Zone> Zones
        {
            get
            {
                return _zones;
            }
        }

        public List<Models.Tdl.Zone> GetNext24Hours(DateTime? fromDate = null)
        {
            if (!fromDate.HasValue)
            {
                fromDate = DateTime.UtcNow;
            }

            var nextDefends = Zones
                .Where(z =>
                        z.NextDefend > fromDate.Value &&
                        z.NextDefend < fromDate.Value.AddDays(1)
                    )
                .OrderBy(z => z.NextDefend)
                .ToList();
            return nextDefends;
        }
    }
}
