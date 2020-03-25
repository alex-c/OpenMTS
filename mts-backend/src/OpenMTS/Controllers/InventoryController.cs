using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
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

        // TODO: comment InventoryController.ctor
        public InventoryController(ILoggerFactory loggerFactory, InventoryService inventoryService)
        {
            Logger = loggerFactory.CreateLogger<InventoryController>();
            InventoryService = inventoryService;
        }

        // TODO: comment InventoryController.GetMaterialBatches
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

        #region Transaction log

        // TODO: comment InventoryController.GetMaterialBatchTransactionLog
        [HttpGet("{batchId}/log")]
        public IActionResult GetMaterialBatchTransactionLog(Guid batchId,
            [FromQuery] bool getAll = false,
            [FromQuery] int page = 0,
            [FromQuery] int elementsPerPage = 50)
        {
            try
            {
                IEnumerable<Transaction> transactions = InventoryService.GetMaterialBatchTransactionLog(batchId);
                IEnumerable<Transaction> paginatedTransactions = transactions.Skip((page - 1) * elementsPerPage).Take(elementsPerPage);
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
