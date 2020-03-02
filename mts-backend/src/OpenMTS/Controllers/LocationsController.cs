using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OpenMTS.Controllers.Contracts.Requests;
using OpenMTS.Models;
using OpenMTS.Services;

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
        /// Gets all available sites.
        /// </summary>
        /// <returns>Returns a list of sites.</returns>
        [HttpGet]
        public IActionResult GetAllStorageSites()
        {
            return Ok(LocationsService.GetAllStorageSites());
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
