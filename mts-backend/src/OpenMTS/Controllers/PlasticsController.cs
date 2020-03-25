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
    /// Endpoint for data access and administration of plastics.
    /// </summary>
    /// <seealso cref="OpenMTS.Controllers.ControllerBase" />
    [Route("api/plastics"), Authorize]
    public class PlasticsController : ControllerBase
    {
        /// <summary>
        /// The service granting access to plastics-related functionality.
        /// </summary>
        /// <value>
        private PlasticsService PlasticsService { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="PlasticsController"/> class.
        /// </summary>
        /// <param name="loggerFactory">A factory to create loggers from.</param>
        public PlasticsController(ILoggerFactory loggerFactory, PlasticsService plasticsService)
        {
            Logger = loggerFactory.CreateLogger<PlasticsController>();
            PlasticsService = plasticsService;
        }

        /// <summary>
        /// Gets paginated plastics.
        /// </summary>
        /// <param name="getAll">Disables pagination - all elements will be returned.</param>
        /// <param name="page">The page to display.</param>
        /// <param name="elementsPerPage">The number of elements to display per page.</param>
        /// <param name="search">An optional search string to filter plastics names with.</param>
        /// <returns>Returns the filtered and paginated plastics.</returns>
        [HttpGet]
        public IActionResult GetPlastics([FromQuery] bool getAll = false, [FromQuery] int page = 0, [FromQuery] int elementsPerPage = 10, [FromQuery] string search = null)
        {
            if (!getAll && (page < 1 || elementsPerPage < 1))
            {
                return HandleBadRequest("Bad pagination parameters.");
            }

            try
            {
                // Get (potentially filtered) plastics and order alphabetically by ID
                IEnumerable<Plastic> plastics = null;
                if (search == null)
                {
                    plastics = PlasticsService.GetPlastics();
                }
                else
                {
                    plastics = PlasticsService.GetPlastics(search);
                }
                plastics = plastics.OrderBy(m => m.Id);

                // Paginate if needed
                IEnumerable<Plastic> paginatedPlastics = plastics;
                if (!getAll)
                {
                    paginatedPlastics = plastics.Skip((page - 1) * elementsPerPage).Take(elementsPerPage);
                }
                
                // Done!
                return Ok(new PaginatedResponse(paginatedPlastics, plastics.Count()));
            }
            catch (Exception exception)
            {
                return HandleUnexpectedException(exception);
            }

        }

        /// <summary>
        /// Creates a new plastic.
        /// </summary>
        /// <param name="plasticCreationRequest">The request to create a new plastic.</param>
        /// <returns>Returns a `201 Created` response on success.</returns>
        [HttpPost, Authorize(Policy = AuthPolicyNames.MAY_CREATE_PLASTIC)]
        public IActionResult CreatePlastic([FromBody] PlasticCreationRequest plasticCreationRequest)
        {
            if (plasticCreationRequest == null ||
                string.IsNullOrWhiteSpace(plasticCreationRequest.Id) ||
                string.IsNullOrWhiteSpace(plasticCreationRequest.Name))
            {
                return HandleBadRequest("A valid plastic creation request with an ID and name must be supplied.");
            }

            try
            {
                Plastic plastic = PlasticsService.CreatePlastic(plasticCreationRequest.Id, plasticCreationRequest.Name);
                return Created(GetNewResourceUri(plasticCreationRequest.Id), plastic);
            }
            catch (PlasticAlreadyExistsException exception)
            {
                return HandleResourceAlreadyExistsException(exception);
            }
            catch (Exception exception)
            {
                return HandleUnexpectedException(exception);
            }
        }

        /// <summary>
        /// Updates a plastic.
        /// </summary>
        /// <param name="id">The ID of the plastic to update.</param>
        /// <param name="plasticUpdateRequest">The data to update.</param>
        /// <returns>Returns the updated plastic.</returns>
        [HttpPatch("{id}"), Authorize(Policy = AuthPolicyNames.MAY_UPDATE_PLASTIC)]
        public IActionResult UpdatePlastic(string id, [FromBody] PlasticUpdateRequest plasticUpdateRequest)
        {
            if (plasticUpdateRequest == null ||
                string.IsNullOrWhiteSpace(plasticUpdateRequest.Name))
            {
                return HandleBadRequest("A valid plastic update request with a name must be supplied.");
            }

            try
            {
                return Ok(PlasticsService.UpdatePlastic(id, plasticUpdateRequest.Name));
            }
            catch (PlasticNotFoundException exception)
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
