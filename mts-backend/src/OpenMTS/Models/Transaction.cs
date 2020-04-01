using System;

namespace OpenMTS.Models
{
    /// <summary>
    /// Represents a transaction (check-in or check-out) on a material batch in inventory for logging purposes.
    /// </summary>
    public class Transaction
    {
        /// <summary>
        /// ID of the transaction;
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// ID of the material batch this transaction is logged for.
        /// </summary>
        public Guid MaterialBatchId { get; set; }

        /// <summary>
        /// Quantity of material moved from the point of view of the storage location.
        /// Negative numbers are for checking out material, positives for checking in material.
        /// </summary>
        public double Quantity { get; set; }

        /// <summary>
        /// Timestamp of the transaction.
        /// </summary>
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// ID of the user who performed the transaction.
        /// </summary>
        public string UserId { get; set; }
    }
}
