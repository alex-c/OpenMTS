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
    /// API route for the management of API keys.
    /// </summary>
    [Route("api/keys"), Authorize(Roles = "0")]
    public class ApiKeyController : ControllerBase
    {
        /// <summary>
        /// The service granting API key functionality.
        /// </summary>
        private ApiKeyService ApiKeyService { get; }

        /// <summary>
        /// Initializes a controller with all needed components.
        /// </summary>
        /// <param name="loggerFactory">A factory to create loggers from.</param>
        /// <param name="apiKeyService">The API key service.</param>
        public ApiKeyController(ILoggerFactory loggerFactory, ApiKeyService apiKeyService)
        {
            ApiKeyService = apiKeyService;
            Logger = loggerFactory.CreateLogger<ApiKeyController>();
        }

        /// <summary>
        /// Gets paginated API keys.
        /// </summary>
        /// <param name="page">Page number to display, starting at 1.</param>
        /// <param name="elementsPerPage">Number of elements to display per page.</param>
        /// <returns>Returns the paginated API keys.</returns>
        [HttpGet]
        public IActionResult GetApiKeys([FromQuery] int page = 1, [FromQuery] int elementsPerPage = 10)
        {
            IEnumerable<ApiKey> keys = ApiKeyService.GetAllApiKeys();
            IEnumerable<ApiKey> paginatedKeys = keys.Skip((page - 1) * elementsPerPage).Take(elementsPerPage);
            return Ok(new PaginatedResponse(paginatedKeys, keys.Count()));
        }

        /// <summary>
        /// Attempts to create a new API key. Upon creation, API keys are disabled and have no access rights.
        /// </summary>
        /// <param name="apiKeyCreationRequest">Te request contract.</param>
        /// <returns>Returns the successfully created key.</returns>
        [HttpPost]
        public IActionResult CreateApiKey([FromBody] ApiKeyCreationRequest apiKeyCreationRequest)
        {
            if (apiKeyCreationRequest == null ||
                string.IsNullOrWhiteSpace(apiKeyCreationRequest.Name))
            {
                return HandleBadRequest("A valid key name has to be supplied.");
            }

            ApiKey key = ApiKeyService.CreateApiKey(apiKeyCreationRequest.Name);
            return Created(GetNewResourceUri(key.Id.ToString()), key);
        }

        /// <summary>
        /// Attempts to update an API key. Allows to change the name and/or access rights.
        /// </summary>
        /// <param name="id">ID of the key to update.</param>
        /// <param name="apiKeyUpdateRequest">Request contract with data to update.</param>
        /// <returns>Returns the updated key on success.</returns>
        [HttpPatch("{id}")]
        public IActionResult UpdateApiKey(Guid id, [FromBody] ApiKeyUpdateRequest apiKeyUpdateRequest)
        {
            if (apiKeyUpdateRequest == null ||
                apiKeyUpdateRequest.Rights == null ||
                string.IsNullOrWhiteSpace(apiKeyUpdateRequest.Name))
            {
                return HandleBadRequest("A valid key name and rights list have to be supplied.");
            }

            // Validate access rights
            IEnumerable<Right> rights;
            try
            {
                rights = apiKeyUpdateRequest.Rights.Select(k => Enum.Parse<Right>(k));
            }
            catch
            {
                return HandleBadRequest("One or more of the submitted rights are no valid right values.");
            }
            
            // Attempt to update the key data.
            try
            {
                ApiKey key = ApiKeyService.UpdateApiKey(id, apiKeyUpdateRequest.Name, rights);
                return Ok(key);
            }
            catch (ApiKeyNotFoundException exception)
            {
                return HandleResourceNotFoundException(exception);
            }
            catch (Exception exception)
            {
                return HandleUnexpectedException(exception);
            }
        }

        /// <summary>
        /// Allows to update an API key's status (whether it is enabled or not).
        /// </summary>
        /// <param name="id">Id of the key to update.</param>
        /// <param name="updateUserStatusRequest">The request contract containing whether the key should be enabled or not.</param>
        /// <returns>Returns `204 No Content` on success.</returns>
        [HttpPut("{id}/status")]
        public IActionResult UpdateApiKeyStatus(Guid id, [FromBody] ApiKeyStatusUpdateRequest apiKeyStatusUpdateRequest)
        {
            if (apiKeyStatusUpdateRequest == null)
            {
                return HandleBadRequest("Missing status data.");
            }

            // Attempt to update status
            try
            {
                ApiKeyService.UpdateApiKeyStatus(id, apiKeyStatusUpdateRequest.Enabled);
                return NoContent();
            }
            catch (ApiKeyNotFoundException exception)
            {
                return HandleResourceNotFoundException(exception);
            }
            catch (Exception exception)
            {
                return HandleUnexpectedException(exception);
            }
        }

        /// <summary>
        /// Deletes an API key.
        /// </summary>
        /// <param name="id">ID of the key to delete.</param>
        /// <returns>Returns a 204 No Content response.</returns>
        [HttpDelete("{id}")]
        public IActionResult DeleteApiKey(Guid id)
        {
            ApiKeyService.DeleteApiKey(id);
            return NoContent();
        }
    }
}
