using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OpenMTS.Authorization;
using OpenMTS.Controllers.Contracts.Requests;
using OpenMTS.Controllers.Contracts.Responses;
using OpenMTS.Models;
using OpenMTS.Services;
using OpenMTS.Services.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OpenMTS.Controllers
{
    /// <summary>
    /// An API route exposing querying and managing options for material batches in the inventory.
    /// </summary>
    /// <seealso cref="OpenMTS.Controllers.ControllerBase" />
    [Route("api/inventory"), Authorize]
    public class InventoryController : ControllerBase
    {
        /// <summary>
        /// The underlying service providing inventory functionality.
        /// </summary>
        private InventoryService InventoryService { get; }

        /// <summary>
        /// A service providing material data.
        /// </summary>
        private MaterialsService MaterialsService { get; }

        /// <summary>
        /// A service providing locations data.
        /// </summary>
        private LocationsService LocationsService { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="InventoryController"/> class.
        /// </summary>
        /// <param name="loggerFactory">A factory to create loggers from.</param>
        /// <param name="inventoryService">The service providing inventory functionality.</param>
        /// <param name="materialsService">A service providing material data.</param>
        /// <param name="locationsService">A service providing locations data.</param>
        public InventoryController(ILoggerFactory loggerFactory,
            InventoryService inventoryService,
            MaterialsService materialsService,
            LocationsService locationsService)
        {
            Logger = loggerFactory.CreateLogger<InventoryController>();
            InventoryService = inventoryService;
            MaterialsService = materialsService;
            LocationsService = locationsService;
        }

        /// <summary>
        /// Gets available material batches, possibly filtered and paginated.
        /// </summary>
        /// <param name="page">The page to display.</param>
        /// <param name="elementsPerPage">The number of elements to display per page.</param>
        /// <param name="materialId">The ID of a material to filter with.</param>
        /// <param name="siteId">The ID of a storage site to filter with.</param>
        /// <returns>Returns the matching material batches.</returns>
        [HttpGet]
        public IActionResult GetMaterialBatches(
            [FromQuery] int page = 0,
            [FromQuery] int elementsPerPage = 10,
            [FromQuery] int? materialId = null,
            [FromQuery] Guid? siteId = null)
        {
            try
            {
                IEnumerable<MaterialBatch> batches = InventoryService.GetMaterialBatches(materialId, siteId);
                IEnumerable<MaterialBatch> paginatedBatches = batches.Skip((page - 1) * elementsPerPage).Take(elementsPerPage);
                return Ok(new PaginatedResponse(paginatedBatches, batches.Count()));
            }
            catch (Exception exception)
            {
                return HandleUnexpectedException(exception);
            }
        }

        /// <summary>
        /// Gets a material batch by its unique ID.
        /// </summary>
        /// <param name="id">The ID of the batch to get.</param>
        /// <returns>Returns the batch.</returns>
        [HttpGet("{id}")]
        public IActionResult GetMaterialBatch(Guid id)
        {
            try
            {
                MaterialBatch batch = InventoryService.GetMaterialBatch(id);
                return Ok(batch);
            }
            catch (MaterialBatchNotFoundException exception)
            {
                return HandleResourceNotFoundException(exception);
            }
            catch (Exception exception)
            {
                return HandleUnexpectedException(exception);
            }
        }

        /// <summary>
        /// Creates a new material batch.
        /// </summary>
        /// <param name="batchCreationRequest">The batch creation request.</param>
        /// <returns>Returns the newly created batch.</returns>
        [HttpPost, Authorize(Policy = AuthPolicyNames.MAY_CREATE_BATCH)]
        public IActionResult CreateMaterialBatch([FromBody] BatchCreationRequest batchCreationRequest)
        {
            if (batchCreationRequest == null ||
                batchCreationRequest.ExpirationDate == null ||
                batchCreationRequest.StorageSiteId == null ||
                batchCreationRequest.StorageAreaId == null ||
                batchCreationRequest.CustomProps == null)
            {
                return HandleBadRequest("Batch data missing for batch creation.");
            }

            // Validate expiration date
            if (batchCreationRequest.ExpirationDate.Date <= DateTime.UtcNow.Date)
            {
                return HandleBadRequest("A batch expiration date must be in the future.");
            }

            // Validate batch number and quantity
            if (batchCreationRequest.BatchNumber <= 0 || batchCreationRequest.Quantity <= 0)
            {
                return HandleBadRequest("The batch number and quantity need to be greater than 0!");
            }

            // Batch locking on creation validation
            bool lockBatch = batchCreationRequest.IsLocked;
            if (lockBatch)
            {
                Role role = GetRole();
                if (role != Role.Administrator && role != Role.ScientificAssistant)
                {
                    return Forbid();
                }
            }

            try
            {
                string userId = GetSubject();

                // Get material
                Material material = MaterialsService.GetMaterial(batchCreationRequest.MaterialId);

                // Get storage location
                StorageSite site = LocationsService.GetStorageSite(batchCreationRequest.StorageSiteId);
                StorageArea area = site.Areas.FirstOrDefault(a => a.Id == batchCreationRequest.StorageAreaId);
                if (area == null)
                {
                    throw new StorageAreaNotFoundException(site.Id, batchCreationRequest.StorageAreaId);
                }
                StorageLocation storageLocation = new StorageLocation(site, area);

                // Proceed with creation and return new batch!
                MaterialBatch batch = InventoryService.CreateMaterialBatch(material,
                    batchCreationRequest.ExpirationDate,
                    storageLocation,
                    batchCreationRequest.BatchNumber,
                    batchCreationRequest.Quantity,
                    batchCreationRequest.CustomProps,
                    lockBatch,
                    userId);
                return Created(GetNewResourceUri(batch), batch);
            }
            catch (MaterialNotFoundException exception)
            {
                return HandleResourceNotFoundException(exception);
            }
            catch (StorageSiteNotFoundException exception)
            {
                return HandleResourceNotFoundException(exception);
            }
            catch (StorageAreaNotFoundException exception)
            {
                return HandleResourceNotFoundException(exception);
            }
            catch (Exception exception)
            {
                return HandleUnexpectedException(exception);
            }
        }

        /// <summary>
        /// Updates an existing material batche's information. Batch quantity cannot be set this way - it can only
        /// be changed by check-in/check-out, or by amending a log entry.
        /// </summary>
        /// <param name="batchId">ID of the batch to update.</param>
        /// <param name="batchUpdateRequest">The data to update.</param>
        /// <returns>Returns the updated batch on success.</returns>
        [HttpPatch("{batchId}"), Authorize(Policy = AuthPolicyNames.MAY_UPDATE_BATCH)]
        public IActionResult UpdateMaterialBatch(Guid batchId, [FromBody] BatchUpdateRequest batchUpdateRequest)
        {
            if (batchUpdateRequest == null ||
                batchUpdateRequest.ExpirationDate == null ||
                batchUpdateRequest.StorageSiteId == null ||
                batchUpdateRequest.StorageAreaId == null ||
                batchUpdateRequest.CustomProps == null)
            {
                return HandleBadRequest("Batch data missing for batch update.");
            }

            // Validate batch number
            if (batchUpdateRequest.BatchNumber <= 0)
            {
                return HandleBadRequest("The batch number must be greater than 0!");
            }

            try
            {
                // Get material
                Material material = MaterialsService.GetMaterial(batchUpdateRequest.MaterialId);

                // Get storage location
                StorageSite site = LocationsService.GetStorageSite(batchUpdateRequest.StorageSiteId);
                StorageArea area = site.Areas.FirstOrDefault(a => a.Id == batchUpdateRequest.StorageAreaId);
                if (area == null)
                {
                    throw new StorageAreaNotFoundException(site.Id, batchUpdateRequest.StorageAreaId);
                }
                StorageLocation storageLocation = new StorageLocation(site, area);

                // Proceed with creation and return new batch!
                MaterialBatch batch = InventoryService.UpdateMaterialBatch(batchId,
                    material,
                    batchUpdateRequest.ExpirationDate,
                    storageLocation,
                    batchUpdateRequest.BatchNumber,
                    batchUpdateRequest.CustomProps);
                return Ok(batch);
            }
            catch (MaterialNotFoundException exception)
            {
                return HandleResourceNotFoundException(exception);
            }
            catch (StorageSiteNotFoundException exception)
            {
                return HandleResourceNotFoundException(exception);
            }
            catch (StorageAreaNotFoundException exception)
            {
                return HandleResourceNotFoundException(exception);
            }
            catch (ArgumentException exception)
            {
                return HandleBadRequest(exception.Message);
            }
            catch (Exception exception)
            {
                return HandleUnexpectedException(exception);
            }
        }

        /// <summary>
        /// Updates the status of a material batch (whether it is locked). Material can't be checked in or out of a locked batch. 
        /// </summary>
        /// <param name="batchId">ID of the batch to lock or unlock.</param>
        /// <param name="batchStatusUpdateRequest">The data to update.</param>
        /// <returns>Returns a `204 No Content` response on success.</returns>
        [HttpPut("{batchId}/status"), Authorize(Policy = AuthPolicyNames.MAY_UPDATE_BATCH_STATUS)]
        public IActionResult UpdateMaterialBatchStatus(Guid batchId, [FromBody] BatchStatusUpdateRequest batchStatusUpdateRequest)
        {
            if (batchStatusUpdateRequest == null)
            {
                return HandleBadRequest("Missing status data.");
            }

            // Attempt to update status
            try
            {
                InventoryService.UpdateBatchStatus(batchId, batchStatusUpdateRequest.IsLocked);
                return NoContent();
            }
            catch (MaterialBatchNotFoundException exception)
            {
                return HandleResourceNotFoundException(exception);
            }
            catch (Exception exception)
            {
                return HandleUnexpectedException(exception);
            }
        }

        #region Transactions & log

        /// <summary>
        /// Gets the paginated transaction log for a specific material batch.
        /// </summary>
        /// <param name="batchId">The ID of the batch.</param>
        /// <param name="getAll">Disables pagination if <c>true</c>.</param>
        /// <param name="page">The page to display.</param>
        /// <param name="elementsPerPage">The number of elements to display per page.</param>
        /// <returns></returns>
        [HttpGet("{batchId}/log")]
        public IActionResult GetMaterialBatchTransactionLog(Guid batchId,
            [FromQuery] bool getAll = false,
            [FromQuery] int page = 0,
            [FromQuery] int elementsPerPage = 50)
        {
            try
            {
                IEnumerable<Transaction> transactions = InventoryService.GetMaterialBatchTransactionLog(batchId);
                IEnumerable<Transaction> paginatedTransactions = transactions;
                if (!getAll)
                {
                    paginatedTransactions = transactions.Skip((page - 1) * elementsPerPage).Take(elementsPerPage);
                }
                return Ok(new PaginatedResponse(paginatedTransactions, transactions.Count()));
            }
            catch (MaterialBatchNotFoundException exception)
            {
                return HandleResourceNotFoundException(exception);
            }
            catch (Exception exception)
            {
                return HandleUnexpectedException(exception);
            }
        }

        /// <summary>
        /// Gets the last transaction from the log of a material batch.
        /// </summary>
        /// <param name="batchId">The ID of the batch to get the last transaction for.</param>
        /// <returns>Returns the last transaction.</returns>
        [HttpGet("{batchId}/last-entry")]
        public IActionResult GetLastMaterialBatchTransactionLogEntry(Guid batchId)
        {
            try
            {
                Transaction transaction = InventoryService.GetLastMaterialBatchTransaction(batchId);
                return Ok(transaction);
            }
            catch (MaterialBatchNotFoundException exception)
            {
                return HandleResourceNotFoundException(exception);
            }
            catch (Exception exception)
            {
                return HandleUnexpectedException(exception);
            }
        }

        /// <summary>
        /// Performs a material transaction: checking material in or out of storage.
        /// </summary>
        /// <param name="batchId">The ID of the batch to perform a transaction on.</param>
        /// <param name="transactionRequest">The transaction request.</param>
        /// <returns>Returns the transcation</returns>
        [HttpPost("{batchId}/log"), Authorize(Policy = AuthPolicyNames.MAY_PERFORM_BATCH_TRANSACTION)]
        public IActionResult PerformMaterialTransaction(Guid batchId, [FromBody] TransactionRequest transactionRequest)
        {
            if (transactionRequest == null)
            {
                return HandleBadRequest("No or invalid transaction data supplied.");
            }

            // Validate quantity value
            if (transactionRequest.Quantity <= 0)
            {
                return HandleBadRequest("The quantity of a material transaction needs to be greater than 0.");
            }

            // Attempt to perform the transaction
            try
            {
                string userId = null;
                if (transactionRequest.UserId == null)
                {
                    userId = GetSubject();
                }
                else
                {
                    userId = transactionRequest.UserId;
                }
                double quantity = transactionRequest.IsCheckout ? transactionRequest.Quantity * -1 : transactionRequest.Quantity;
                Transaction transaction = InventoryService.PerformMaterialTransaction(batchId, quantity, userId);
                return Ok(transaction);
            }
            catch (MaterialBatchNotFoundException exception)
            {
                return HandleResourceNotFoundException(exception);
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (ArgumentException exception)
            {
                return HandleBadRequest(exception.Message);
            }
            catch (Exception exception)
            {
                return HandleUnexpectedException(exception);
            }
        }

        /// <summary>
        /// Attempts to amend the last transaction of a material batch.
        /// </summary>
        /// <param name="batchId">ID of the batch to amend the last transaction for.</param>
        /// <param name="transactionId">ID of the transaction to amend.</param>
        /// <param name="transactionLogEntryAmendingRequest">The data to amend.</param>
        /// <returns>Returns a `204 No Content` result on success.</returns>
        [HttpPatch("{batchId}/log/{transactionId}")]
        public IActionResult AmendLastMaterialBatchTransactionLogEntry(Guid batchId, Guid transactionId, [FromBody] TransactionLogEntryAmendingRequest transactionLogEntryAmendingRequest)
        {
            if (transactionLogEntryAmendingRequest == null)
            {
                return HandleBadRequest("Missing amending data.");
            }

            try
            {
                string userId = GetSubject();
                InventoryService.AmendLastMaterialBatchTransaction(batchId, transactionId, transactionLogEntryAmendingRequest.Quantity, userId);
                return NoContent();
            }
            catch (MaterialBatchNotFoundException exception)
            {
                return HandleResourceNotFoundException(exception);
            }
            catch (NotLastLogEntryException exception)
            {
                return HandleBadRequest(exception.Message);
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (Exception exception)
            {
                return HandleUnexpectedException(exception);
            }
        }

        #endregion
    }
}
