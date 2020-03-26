using System;
using System.Collections.Generic;

namespace OpenMTS.Models
{
    /// <summary>
    /// A batch of material in the inventory.
    /// </summary>
    public class MaterialBatch
    {
        /// <summary>
        /// ID of the batch.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// The material constituting this batch.
        /// </summary>
        public Material Material { get; set; }

        /// <summary>
        /// Storage location of the batch.
        /// </summary>
        public StorageLocation StorageLocation { get; set; }

        /// <summary>
        /// Batch number.
        /// </summary>
        public long BatchNumber { get; set; }

        /// <summary>
        /// Expiration date of the material.
        /// </summary>
        public DateTime ExpirationDate { get; set; }

        /// <summary>
        /// Whether the batch has been locked.
        /// </summary>
        public bool IsLocked { get; set; }

        /// <summary>
        /// Quantity of material in the batch.
        /// </summary>
        public double Quantity { get; set; }

        /// <summary>
        /// Values of custom material batch properties.
        /// </summary>
        public Dictionary<Guid, string> CustomProps { get; set; }
    }
}
