using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OpenMTS.Controllers.Contracts.Responses;
using System;

namespace OpenMTS.Controllers
{
    /// <summary>
    /// Base class for controllers, that helps to handle logging and errors in a unified way.
    /// </summary>
    public class ControllerBase : Controller
    {
        /// <summary>
        /// Logger for controller-level logging.
        /// </summary>
        protected ILogger Logger { get; set; }

        #region Error Handling

        /// <summary>
        /// Handle bad requests.
        /// </summary>
        /// <param name="message">Message that should explain why the request is bad!</param>
        /// <returns>Returns a 400 error.</returns>
        protected IActionResult HandleBadRequest(string message)
        {
            return BadRequest(new ClientErrorResponse(message));
        }

        /// <summary>
        /// Handle unexpected exceptions.
        /// </summary>
        /// <param name="exception">Unexpected exception that was caught.</param>
        /// <returns>Returns a 500 error.</returns>
        protected IActionResult HandleUnexpectedException(Exception exception)
        {
            Logger?.LogError(exception, "An unexpected exception was caught.");
            return new StatusCodeResult(500);
        }

        /// <summary>
        /// Handle unexpected exceptions with extra message.
        /// </summary>
        /// <param name="exception">Unexpected exception that was caught.</param>
        /// <param name="message">Extra message explaining the problem.</param>
        /// <returns>Returns a 500 error.</returns>
        protected IActionResult HandleUnexpectedException(Exception exception, string message)
        {
            Logger?.LogError(exception, message);
            return new StatusCodeResult(500);
        }

        #endregion
    }
}
