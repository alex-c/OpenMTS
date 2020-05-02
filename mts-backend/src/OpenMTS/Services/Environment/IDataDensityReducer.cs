using OpenMTS.Models.Environmnt;
using System.Collections.Generic;

namespace OpenMTS.Services.Environment
{
    /// <summary>
    /// A generic interface for a data density reducer for environmental data.
    /// </summary>
    public interface IDataDensityReducer
    {
        /// <summary>
        /// Reduces the data density in order to return less than <paramref name="maxPoint"/> data points.
        /// </summary>
        /// <param name="data">The source dataset to reduce.</param>
        /// <param name="maxPoint">The upper bound of points for the result dataset.</param>
        /// <returns>Returns a reduced dataset.</returns>
        IEnumerable<DataPoint> ReduceDensity(IEnumerable<DataPoint> data, int maxPoint);
    }
}
