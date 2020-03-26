using Microsoft.Extensions.Logging;
using OpenMTS.Models;
using OpenMTS.Repositories;
using OpenMTS.Services.Exceptions;
using System;
using System.Collections.Generic;

namespace OpenMTS.Services
{
    /// <summary>
    /// A service for querying and managing the inventory.
    /// </summary>
    public class InventoryService
    {
        /// <summary>
        /// Provides transaction logging functionality.
        /// </summary>
        private TransactionLogService TransactionLogService { get; }

        /// <summary>
        /// The underlying repository for material batches.
        /// </summary>
        private IMaterialBatchRepository MaterialBatchRepository { get; }

        /// <summary>
        /// A logger instance for local logging needs
        /// </summary>
        private ILogger Logger { get; }

        public InventoryService(ILoggerFactory loggerFactory,
            IMaterialBatchRepository materialBatchRepository,
            TransactionLogService transactionLogService)
        {
            Logger = loggerFactory.CreateLogger<InventoryService>();
            MaterialBatchRepository = materialBatchRepository;
            TransactionLogService = transactionLogService;
        }

        /// <summary>
        /// Gets material batches, possibly filtered.
        /// </summary>
        /// <param name="materialId">The ID of a material to filter with.</param>
        /// <param name="siteId">The ID of a storage site to filter with.</param>
        /// <returns>Returns all matching batches.</returns>
        public IEnumerable<MaterialBatch> GetMaterialBatches(int? materialId = null, Guid? siteId = null)
        {
            IEnumerable<MaterialBatch> materialBatches = MaterialBatchRepository.GetMaterialBatches(materialId, siteId);
            return materialBatches;
        }

        /// <summary>
        /// Gets a material batch by its unique ID.
        /// </summary>
        /// <param name="batchId">The ID of the batch to get.</param>
        /// <exception cref="MaterialBatchNotFoundException">Thrown if no matching batch could be found.</exception>
        /// <returns>Returns the batch.</returns>
        public MaterialBatch GetMaterialBatch(Guid batchId)
        {
            return GetBatchOrThrowNotFoundException(batchId);
        }

        /// <summary>
        /// Gets the full transaction log of a material batch.
        /// </summary>
        /// <param name="batchId">The ID of the batch to get the log for..</param>
        /// <exception cref="MaterialBatchNotFoundException">Thrown if no matching batch could be found.</exception>
        /// <returns>Returns all matching transactions.</returns>
        public IEnumerable<Transaction> GetMaterialBatchTransactionLog(Guid batchId)
        {
            GetBatchOrThrowNotFoundException(batchId);
            return TransactionLogService.GetTransactionLog(batchId);
        }

        /// <summary>
        /// Performs a material transaction: checking material in or out of storage.
        /// </summary>
        /// <param name="batchId">The ID of the batch to perform a transaction on.</param>
        /// <param name="quantity">The quantity to check out or in. Negative numbers indicate a check-out, positive numbers a check-in.</param>
        /// <param name="userId">The ID of the user performing the transaction.</param>
        /// <returns>Returns the transcation.</returns>
        /// <exception cref="ArgumentException">Thrown if the new computed quantity of material is less than 0.</exception>
        public Transaction PerformMaterialTransaction(Guid batchId, double quantity, string userId)
        {
            MaterialBatch batch = GetBatchOrThrowNotFoundException(batchId);

            // Compute and validate new quantity
            double newQuantity = Math.Round(batch.Quantity + quantity, 3, MidpointRounding.AwayFromZero);
            if (newQuantity < 0)
            {
                throw new ArgumentException("The quantity of a batch cannot be less than 0. You cannot check out more material than there is in the inventory!");
            }

            // Update quantity
            batch.Quantity = newQuantity;

            // Generate transaction
            Transaction transaction = new Transaction()
            {
                Id = Guid.NewGuid(),
                MaterialBatchId = batchId,
                Quantity = quantity,
                Timestamp = DateTime.UtcNow,
                UserId = userId
            };

            // Persist batch and log transaction
            MaterialBatchRepository.UpdateMaterialBatch(batch);
            TransactionLogService.LogTransaction(transaction);

            // Done - return transaction!
            return transaction;
        }

        #region Private Helpers

        /// <summary>
        /// Attempts to get a batch from the underlying repository and throws a <see cref="MaterialBatchNotFoundException"/> if no matching batch could be found.
        /// </summary>
        /// <param name="id">ID of the material batch to get.</param>
        /// <exception cref="MaterialBatchNotFoundException">Thrown if no matching batch could be found.</exception>
        /// <returns>Returns the batch, if found.</returns>
        private MaterialBatch GetBatchOrThrowNotFoundException(Guid id)
        {
            MaterialBatch batch = MaterialBatchRepository.GetMaterialBatch(id);

            // Check for batch existence
            if (batch == null)
            {
                throw new MaterialBatchNotFoundException(id);
            }

            return batch;
        }

        #endregion
    }
}
