using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OpenMTS.Models.Trace;
using OpenMTS.Services;
using OpenMTS.Services.Exceptions;
using System;

namespace OpenMTS.Controllers
{
    /// <summary>
    /// Endpoint for tracing of material data from transactions.
    /// </summary>
    /// <seealso cref="OpenMTS.Controllers.ControllerBase" />
    [Route("api/trace"), Authorize]
    public class TraceController : ControllerBase
    {
        /// <summary>
        /// Provides the tracing functionality.
        /// </summary>
        private TraceService TraceService { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="TraceController"/> class.
        /// </summary>
        /// <param name="loggerFactory">A factory to create loggers from.</param>
        /// <param name="traceService">The trace service.</param>
        public TraceController(ILoggerFactory loggerFactory, TraceService traceService)
        {
            Logger = loggerFactory.CreateLogger<TraceController>();
            TraceService = traceService;
        }

        /// <summary>
        /// Traces material data from a check-out transaction.
        /// </summary>
        /// <param name="transactionId">The ID of the transaction to trace from.</param>
        /// <returns>Returns the trace result.</returns>
        [HttpGet("{transactionId}")]
        public IActionResult Trace(Guid transactionId)
        {
            try
            {
                TraceResult result = TraceService.Trace(transactionId);
                return Ok(result);
            }
            catch (TransactionNotFoundException exception)
            {
                return HandleResourceNotFoundException(exception);
            }
            catch (MaterialBatchNotFoundException exception)
            {
                return HandleResourceNotFoundException(exception);
            }
            catch (MaterialNotFoundException exception)
            {
                return HandleResourceNotFoundException(exception);
            }
            catch (Exception exception)
            {
                return HandleUnexpectedException(exception);
            }
        }
    }
}
