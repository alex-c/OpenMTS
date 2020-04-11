using System;

namespace OpenMTS.Models.Environmnt
{
    /// <summary>
    /// Represents a single environemntal data point: the temperature or humidity of a storage site at a specific time.
    /// </summary>
    public class DataPoint
    {
        /// <summary>
        /// The timestamp at which the data point was recorded.
        /// </summary>
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// The recorded value.
        /// </summary>
        public double Value { get; set; }
    }
}
