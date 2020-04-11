using OpenMTS.Models;
using OpenMTS.Models.Environmnt;
using System;
using System.Collections.Generic;

namespace OpenMTS.Repositories
{
    /// <summary>
    /// A generic interface for a repository of environmental data.
    /// </summary>
    public interface IEnvironmentalDataRepository
    {
        /// <summary>
        /// Gets the latest value for a specific storage site and environmental factor.
        /// </summary>
        /// <param name="site">The storage site to get the latest value for.</param>
        /// <param name="factor">The factor to get the latest value for.</param>
        /// <returns>Returns the latest value, or null.</returns>
        DataPoint GetLatestValue(StorageSite site, EnvironmentalFactor factor);

        /// <summary>
        /// Gets the history of a specific storage site and environmental factor.
        /// </summary>
        /// <param name="site">The storage site to get the history for.</param>
        /// <param name="factor">The factor to get the history for.</param>
        /// <param name="startTime">Start time of the period to get value for.</param>
        /// <param name="endTime">End time of the period to get value for.</param>
        /// <returns>Returns the matching values.</returns>
        IEnumerable<DataPoint> GetHistory(StorageSite site, EnvironmentalFactor factor, DateTime startTime, DateTime endTime);

        /// <summary>
        /// Gets the min and max values for a specific storage site and environmental factor.
        /// </summary>
        /// <param name="site">The storage site to get the history for.</param>
        /// <param name="factor">The factor to get the history for.</param>
        /// <param name="startTime">Start time of the period to get value for.</param>
        /// <param name="endTime">End time of the period to get value for.</param>
        /// <returns>Returns the extrema.</returns>
        Extrema GetExtrema(StorageSite site, EnvironmentalFactor factor, DateTime startTime, DateTime endTime);

        /// <summary>
        /// Records an environmental snapshot.
        /// </summary>
        /// <param name="snapshot">The snapshot to record.</param>
        void RecordEnvironmentalSnapshot(EnvironmentSnapshot snapshot);
    }
}
