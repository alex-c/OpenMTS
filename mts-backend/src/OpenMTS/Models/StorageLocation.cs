using System;

namespace OpenMTS.Models
{
    /// <summary>
    /// Aggregates the storage site and area information for material batches.
    /// </summary>
    public class StorageLocation
    {
        /// <summary>
        /// ID of the storage site.
        /// </summary>
        public Guid StorageSiteId { get; set; }

        /// <summary>
        /// ID of the storage area.
        /// </summary>
        public Guid StorageAreaId { get; set; }

        /// <summary>
        /// Name of the storage site.
        /// </summary>
        public string StorageSiteName { get; set; }

        /// <summary>
        /// Name of the storage area.
        /// </summary>
        public string StorageAreaName { get; set; }
    }
}
