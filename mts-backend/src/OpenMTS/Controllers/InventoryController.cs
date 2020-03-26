using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
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
        /// Initializes a new instance of the <see cref="InventoryController"/> class.
        /// </summary>
        /// <param name="loggerFactory">A factory to create loggers from.</param>
        /// <param name="inventoryService">The service providing inventory functionality.</param>
        public InventoryController(ILoggerFactory loggerFactory, InventoryService inventoryService)
        {
            Logger = loggerFactory.CreateLogger<InventoryController>();
            InventoryService = inventoryService;
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

        #region Transaction log

        /// <summary>
        /// Performs a material transaction: checking material in or out of storage.
        /// </summary>
        /// <param name="id">The ID of the batch to perform a transaction on.</param>
        /// <param name="transactionRequest">The transaction request.</param>
        /// <returns>Returns the transcation</returns>
        [HttpPost("{id}/log")] // TODO: auth policy
        public IActionResult PerformMaterialTransaction(Guid id, [FromBody] TransactionRequest transactionRequest)
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
                string userId = GetSubject();
                double quantity = transactionRequest.IsCheckout ? transactionRequest.Quantity * -1 : transactionRequest.Quantity;
                Transaction transaction = InventoryService.PerformMaterialTransaction(id, quantity, userId);
                return Ok(transaction);
            }
            catch (MaterialBatchNotFoundException exception)
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

        #endregion
    }
}
