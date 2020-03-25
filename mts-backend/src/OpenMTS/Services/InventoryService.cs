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

        // TODO: comment InventoryService.GetMaterialBatches
        public IEnumerable<MaterialBatch> GetMaterialBatches(int? materialId = null, Guid? siteId = null)
        {
            IEnumerable<MaterialBatch> materialBatches = MaterialBatchRepository.GetMaterialBatches(materialId, siteId);
            return materialBatches;
        }

        // TODO: comment InventoryService.GetMaterialBatchTransactionLog
        public IEnumerable<Transaction> GetMaterialBatchTransactionLog(Guid id)
        {
            MaterialBatch batch = GetBatchOrThrowNotFoundException(id);
            return TransactionLogService.GetTransactionLog(id);
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
