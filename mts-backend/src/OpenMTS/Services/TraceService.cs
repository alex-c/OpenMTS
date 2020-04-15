using Microsoft.Extensions.Logging;
using OpenMTS.Models;
using OpenMTS.Models.Environmnt;
using OpenMTS.Models.Trace;
using System;
using System.Linq;

namespace OpenMTS.Services
{
    /// <summary>
    /// Provides tracing of material data from transactions.
    /// </summary>
    public class TraceService
    {
        // Other services needed for tracing
        private TransactionLogService TransactionLogService { get; }
        private InventoryService InventoryService { get; }
        private MaterialsService MaterialsService { get; }
        private EnvironmentService EnvironmentService { get; }

        /// <summary>
        /// For local logging needs.
        /// </summary>
        private ILogger Logger { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="TraceService"/> class.
        /// </summary>
        /// <param name="loggerFactory">A factory to create loggers from..</param>
        /// <param name="transactionLogService">The transaction log service.</param>
        /// <param name="inventoryService">The inventory service.</param>
        /// <param name="materialsService">The materials service.</param>
        /// <param name="environmentService">The environment service.</param>
        public TraceService(ILoggerFactory loggerFactory,
            TransactionLogService transactionLogService,
            InventoryService inventoryService,
            MaterialsService materialsService,
            EnvironmentService environmentService)
        {
            Logger = loggerFactory.CreateLogger<TraceService>();
            TransactionLogService = transactionLogService;
            InventoryService = inventoryService;
            MaterialsService = materialsService;
            EnvironmentService = environmentService;
        }

        /// <summary>
        /// Retrieves all related material data from a transaction.
        /// </summary>
        /// <typeparam name="Guid">ID of the transaction to use as a starting point.</typeparam>
        /// <returns>Returns a trace result on success.</returns>
        /// <exception cref="Exceptions.TransactionNotFoundException" />
        /// <exception cref="Exceptions.MaterialBatchNotFoundException" />
        /// <exception cref="Exceptions.MaterialNotFoundException" />
        public TraceResult Trace(Guid transactionId)
        {
            // Get checkout transaction
            Transaction checkOutTransaction = TransactionLogService.GetTransaction(transactionId);

            // Get checkin transaction
            Transaction checkInTransaction = TransactionLogService.GetTransactionLog(checkOutTransaction.MaterialBatchId).Last();

            // Get batch
            MaterialBatch batch = InventoryService.GetMaterialBatch(checkOutTransaction.MaterialBatchId);

            // Get and replace material data in extra step, since the InventoryService currently doesn't return custom material prop values
            Material material = MaterialsService.GetMaterial(batch.Material.Id);
            batch.Material = material;

            // Get temperature extrema
            Extrema temperature = EnvironmentService.GetExtrema(batch.StorageLocation.StorageSiteId,
                EnvironmentalFactor.Temperature,
                checkInTransaction.Timestamp,
                checkOutTransaction.Timestamp);

            // Get humidity extrema
            Extrema humidity = EnvironmentService.GetExtrema(batch.StorageLocation.StorageSiteId,
                EnvironmentalFactor.Humidity,
                checkInTransaction.Timestamp,
                checkOutTransaction.Timestamp);

            // Return trace result object
            return new TraceResult()
            {
                Batch = batch,
                CheckInTransaction = checkInTransaction,
                CheckOutTransaction = checkOutTransaction,
                Temperature = temperature,
                Humidity = humidity
            };
        }
    }
}
