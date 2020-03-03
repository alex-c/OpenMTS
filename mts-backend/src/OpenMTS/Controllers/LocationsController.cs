using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OpenMTS.Controllers.Contracts.Requests;
using OpenMTS.Controllers.Contracts.Responses;
using OpenMTS.Models;
using OpenMTS.Services;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OpenMTS.Controllers
{
    /// <summary>
    /// Route for getting and administrationg storage locations: storage sites and areas.
    /// </summary>
    [Route("api/sites")]
    public class LocationsController : ControllerBase
    {
        /// <summary>
        /// The service granting locations-related functionality.
        /// </summary>
        private LocationsService LocationsService { get; }

        /// <summary>
        /// Injects all needed components.
        /// </summary>
        /// <param name="loggerFactory">A factory to create loggers from.</param>
        /// <param name="locationsService">A service for locations-related functionality.</param>
        public LocationsController(ILoggerFactory loggerFactory, LocationsService locationsService)
        {
            Logger = loggerFactory.CreateLogger<LocationsController>();
            LocationsService = locationsService;
        }

        /// <summary>
        /// Gets available sites, paginated.
        /// </summary>
        /// <param name="page">Page to display, starting at 1.</param>
        /// <param name="elementsPerPage">Elements to display per page.</param>
        /// <param name="search">String to filter site names with.</param>
        /// <returns>Returns a paginated list of sites.</returns>
        [HttpGet]
        public IActionResult GetStorageSites([FromQuery] int page = 1, [FromQuery] int elementsPerPage = 10, [FromQuery] string search = null)
        {
            try
            {
                IEnumerable<StorageSite> sites = null;
                if (string.IsNullOrWhiteSpace(search))
                {
                    sites = LocationsService.GetAllStorageSites();
                }
                else
                {
                    sites = LocationsService.SearchStorageSitesByName(search);
                }
                IEnumerable<StorageSite> paginatedSites = sites.Skip((page - 1) * elementsPerPage).Take(elementsPerPage);
                return Ok(new PaginatedResponse(paginatedSites, sites.Count()));
            }
            catch (Exception exception)
            {
                return HandleUnexpectedException(exception);
            }
        }

        /// <summary>
        /// Creates a new storage site.
        /// </summary>
        /// <param name="storageSiteCreationRequest">The dat needed for creating a site.</param>
        /// <returns>Returns a `201 Created` response.</returns>
        [HttpPost]
        public IActionResult CreateStorageSite([FromBody] StorageSiteCreationRequest storageSiteCreationRequest)
        {
            StorageSite site = LocationsService.CreateStorageSite(storageSiteCreationRequest.Name);
            return Created(GetNewResourceUri(site.Id), site);
        }
    }
}
