using System;
using System.Collections.Generic;

namespace OpenMTS.Models
{
    /// <summary>
    /// A storage site, which can contain several storage areas.
    /// </summary>
    public class StorageSite
    {
        /// <summary>
        /// ID of the storage site.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Name of the storage site.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Areas associated with this storage site.
        /// </summary>
        public ICollection<StorageArea> Areas { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="StorageSite"/> class.
        /// </summary>
        public StorageSite()
        {
            Areas = new List<StorageArea>();
        }
    }
}
