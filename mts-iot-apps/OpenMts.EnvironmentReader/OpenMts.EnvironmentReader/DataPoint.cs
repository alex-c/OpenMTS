using System;

namespace OpenMts.EnvironmentReader
{
    /// <summary>
    /// Represents an environmental data point for a specific storage site at a specific timestamp.
    /// </summary>
    public class DataPoint
    {
        /// <summary>
        /// The timestamp this data point was recorded at.
        /// </summary>
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// The recorded temeprature.
        /// </summary>
        public double? Temperature { get; set; }

        /// <summary>
        /// The recorded humidity.
        /// </summary>
        public double? Humidity { get; set; }
    }
}
