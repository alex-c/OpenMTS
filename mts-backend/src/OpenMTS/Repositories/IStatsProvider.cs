using OpenMTS.Models.Stats;
using System.Collections.Generic;

namespace OpenMTS.Repositories
{
    /// <summary>
    /// A provider of statistics that can't be provided by the model repositories.
    /// </summary>
    public interface IStatsProvider
    {
        /// <summary>
        /// Gets an overview of all available storage sites, which includes latest environmental values and total material.
        /// </summary>
        /// <returns>Returns the overview of each storage.</returns>
        IEnumerable<StorageSiteOverview> GetSitesOverview();
    }
}
