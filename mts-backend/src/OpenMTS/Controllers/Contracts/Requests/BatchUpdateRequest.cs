using System;
using System.Collections.Generic;

namespace OpenMTS.Controllers.Contracts.Requests
{
    /// <summary>
    /// A contract for a request to update an existing material batch.
    /// </summary>
    public class BatchUpdateRequest
    {
        /// <summary>
        /// ID of the material the batch is composed of.
        /// </summary>
        public int MaterialId { get; set; }

        /// <summary>
        /// Expiration date of the material.
        /// </summary>
        public DateTime ExpirationDate { get; set; }

        /// <summary>
        /// ID of the storage site the material is stored at.
        /// </summary>
        public Guid StorageSiteId { get; set; }

        /// <summary>
        /// ID of the storage area the material is stored at.
        /// </summary>
        public Guid StorageAreaId { get; set; }

        /// <summary>
        /// The manufacturer provided batch number.
        /// </summary>
        public long BatchNumber { get; set; }

        /// <summary>
        /// A mapping of custom property IDs and values.
        /// </summary>
        public Dictionary<Guid, string> CustomProps { get; set; }
    }
}
