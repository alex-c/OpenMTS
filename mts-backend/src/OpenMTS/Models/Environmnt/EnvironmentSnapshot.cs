using System;

namespace OpenMTS.Models.Environmnt
{
    /// <summary>
    /// Represents a single environemntal data point: the temperature and/or humidity of a storage site at a specific time.
    /// </summary>
    public class EnvironmentSnapshot
    {
        /// <summary>
        /// ID of the storage site this snapshot is for.
        /// </summary>
        public Guid StorageSiteId { get; set; }

        /// <summary>
        /// The timestamp at which the data point was recorded.
        /// </summary>
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// The recorded temperature, if any.
        /// </summary>
        public double? Temperature { get; set; }

        /// <summary>
        /// The recorded humidity, if any.
        /// </summary>
        public double? Humidity { get; set; }
    }
}
