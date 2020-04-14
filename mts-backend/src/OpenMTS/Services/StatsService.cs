using OpenMTS.Models.Environmnt;
using OpenMTS.Models.Stats;
using OpenMTS.Repositories;
using System.Collections.Generic;

namespace OpenMTS.Services
{
    /// <summary>
    /// Provides various information and statistics.
    /// </summary>
    public class StatsService
    {
        /// <summary>
        /// Provider of stats-related data that the model repositories can't provide.
        /// </summary>
        private IStatsProvider Provider { get; }

        /// <summary>
        /// Source of environmental data.
        /// </summary>
        private IEnvironmentalDataRepository EnvironmentalDataRepository { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="StatsService"/> class.
        /// </summary>
        /// <param name="provider">The stats-related data provider..</param>
        /// <param name="environmentalDataRepository">The environmental data repository.</param>
        public StatsService(IStatsProvider provider, IEnvironmentalDataRepository environmentalDataRepository)
        {
            Provider = provider;
            EnvironmentalDataRepository = environmentalDataRepository;
        }

        /// <summary>
        /// Gets an overview of all available storage sites, which includes latest environmental values and total material.
        /// </summary>
        /// <returns>Returns the overview of each storage.</returns>
        public IEnumerable<StorageSiteOverview> GetSitesOverview()
        {
            IEnumerable<StorageSiteOverview> sites = Provider.GetSitesOverview();
            foreach (StorageSiteOverview site in sites)
            {
                site.Temperature = EnvironmentalDataRepository.GetLatestValue(site.Site, EnvironmentalFactor.Temperature)?.Value;
                site.Humidity = EnvironmentalDataRepository.GetLatestValue(site.Site, EnvironmentalFactor.Humidity)?.Value;
            }
            return sites;
        }
    }
}
