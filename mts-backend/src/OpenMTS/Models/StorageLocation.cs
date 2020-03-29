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

        /// <summary>
        /// Initializes a new instance of the <see cref="StorageLocation"/> class.
        /// </summary>
        public StorageLocation() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="StorageLocation"/> class.
        /// </summary>
        /// <param name="site">The storage site.</param>
        /// <param name="area">The storage area.</param>
        public StorageLocation(StorageSite site, StorageArea area)
        {
            StorageSiteId = site.Id;
            StorageAreaId = area.Id;
            StorageSiteName = site.Name;
            StorageAreaName = area.Name;
        }
    }
}
