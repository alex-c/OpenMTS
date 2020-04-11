using Confluent.Kafka;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OpenMTS.Authorization;
using OpenMTS.Controllers.Contracts.Requests;
using OpenMTS.Controllers.Contracts.Responses;
using OpenMTS.Models;
using OpenMTS.Models.Environmnt;
using OpenMTS.Services;
using OpenMTS.Services.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OpenMTS.Controllers
{
    /// <summary>
    /// Route for getting and administrationg storage locations: storage sites and areas.
    /// </summary>
    [Route("api/sites"), Authorize]
    public class LocationsController : ControllerBase
    {
        /// <summary>
        /// The service granting locations-related functionality.
        /// </summary>
        private LocationsService LocationsService { get; }

        /// <summary>
        /// Provides storage site environment data.
        /// </summary>
        private EnvironmentService EnvironmentService { get; }

        /// <summary>
        /// Injects all needed components.
        /// </summary>
        /// <param name="loggerFactory">A factory to create loggers from.</param>
        /// <param name="locationsService">A service for locations-related functionality.</param>
        public LocationsController(ILoggerFactory loggerFactory, LocationsService locationsService, EnvironmentService environmentService)
        {
            Logger = loggerFactory.CreateLogger<LocationsController>();
            LocationsService = locationsService;
            EnvironmentService = environmentService;
        }

        /// <summary>
        /// Gets available sites, paginated.
        /// </summary>
        /// <param name="getAll">Disables pagination - all elements will be returned.</param>
        /// <param name="page">Page to display, starting at 1.</param>
        /// <param name="elementsPerPage">Elements to display per page.</param>
        /// <param name="search">String to filter site names with.</param>
        /// <returns>Returns a paginated list of sites.</returns>
        [HttpGet]
        public IActionResult GetStorageSites(
            [FromQuery] bool getAll = false,
            [FromQuery] int page = 1,
            [FromQuery] int elementsPerPage = 10,
            [FromQuery] string search = null)
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
                IEnumerable<StorageSite> paginatedSites = sites;
                if (!getAll)
                {
                    paginatedSites = sites.Skip((page - 1) * elementsPerPage).Take(elementsPerPage);
                }
                return Ok(new PaginatedResponse(paginatedSites, sites.Count()));
            }
            catch (Exception exception)
            {
                return HandleUnexpectedException(exception);
            }
        }

        /// <summary>
        /// Attempts to get a storage site by it's ID.
        /// </summary>
        /// <returns>Rerturns the storage site, if found..</returns>
        [HttpGet("{id}")]
        public IActionResult GetStorageSite(Guid id)
        {
            try
            {
                return Ok(LocationsService.GetStorageSite(id));
            }
            catch (StorageSiteNotFoundException exception)
            {
                return HandleResourceNotFoundException(exception);
            }
            catch (Exception exception)
            {
                return HandleUnexpectedException(exception);
            }
        }

        #region Environment data

        /// <summary>
        /// Gets the last recorded temperature for a storage site.
        /// </summary>
        /// <param name="id">ID of the storage site.</param>
        /// <param name="factor">The factor to get data for.</param>
        /// <returns>Returns the last recorded value.</returns>
        [HttpGet("{id}/temperature")]
        public IActionResult GetStorageSiteTemperature(Guid id)
        {
            return GetStorageSiteEnvironementalFactor(id, EnvironmentalFactor.Temperature);
        }

        /// <summary>
        /// Gets the temperature history of a storage site.
        /// </summary>
        /// <param name="id">ID of the storage site.</param>
        /// <param name="factor">The factor to get data for.</param>
        /// <param name="startTime">The history start time.</param>
        /// <param name="endTime">The history end time.</param>
        /// <returns>Returns the history.</returns>
        [HttpGet("{id}/temperature/history")]
        public IActionResult GetStorageSiteTemperatureHistory(Guid id, [FromQuery] DateTime startTime, [FromQuery] DateTime endTime)
        {
            return GetStorageSiteEnvironementalFactorHistory(id, EnvironmentalFactor.Temperature, startTime, endTime);
        }

        /// <summary>
        /// Gets the min and max recorded temperature for a storage site.
        /// </summary>
        /// <param name="id">ID of the storage site.</param>
        /// <param name="factor">The factor to get data for.</param>
        /// <param name="startTime">The period start time.</param>
        /// <param name="endTime">The period end time.</param>
        /// <returns>Returns the maxima.</returns>
        [HttpGet("{id}/temperature/extrema")]
        public IActionResult GetStorageSiteTemperatureExtrema(Guid id, [FromQuery] DateTime startTime, [FromQuery] DateTime endTime)
        {
            return GetStorageSiteEnvironementalFactorExtrema(id, EnvironmentalFactor.Temperature, startTime, endTime);
        }

        /// <summary>
        /// Gets the last recorded humidity for a storage site.
        /// </summary>
        /// <param name="id">ID of the storage site.</param>
        /// <param name="factor">The factor to get data for.</param>
        /// <returns>Returns the last recorded value.</returns>
        [HttpGet("{id}/humidity")]
        public IActionResult GetStorageSiteHumidity(Guid id)
        {
            return GetStorageSiteEnvironementalFactor(id, EnvironmentalFactor.Humidity);
        }

        /// <summary>
        /// Gets the humidity history of a storage site.
        /// </summary>
        /// <param name="id">ID of the storage site.</param>
        /// <param name="factor">The factor to get data for.</param>
        /// <param name="startTime">The history start time.</param>
        /// <param name="endTime">The history end time.</param>
        /// <returns>Returns the history.</returns>
        [HttpGet("{id}/humidity/history")]
        public IActionResult GetStorageSiteHumidityHistory(Guid id, [FromQuery] DateTime startTime, [FromQuery] DateTime endTime)
        {
            return GetStorageSiteEnvironementalFactorHistory(id, EnvironmentalFactor.Humidity, startTime, endTime);
        }

        /// <summary>
        /// Gets the min and max recorded temperature for a storage site.
        /// </summary>
        /// <param name="id">ID of the storage site.</param>
        /// <param name="factor">The factor to get data for.</param>
        /// <param name="startTime">The period start time.</param>
        /// <param name="endTime">The period end time.</param>
        /// <returns>Returns the maxima.</returns>
        [HttpGet("{id}/humidity/extrema")]
        public IActionResult GetStorageSiteHumidityExtrema(Guid id, [FromQuery] DateTime startTime, [FromQuery] DateTime endTime)
        {
            return GetStorageSiteEnvironementalFactorExtrema(id, EnvironmentalFactor.Humidity, startTime, endTime);
        }

        /// <summary>
        /// Gets the last value for a storage site environemental factor.
        /// </summary>
        /// <param name="id">ID of the storage site.</param>
        /// <param name="factor">The factor to get data for.</param>
        /// <returns>Returns the last recorded value.</returns>
        private IActionResult GetStorageSiteEnvironementalFactor(Guid id, EnvironmentalFactor factor)
        {
            try
            {
                DataPoint lastValue = EnvironmentService.GetLatestValue(id, factor);
                return Ok(lastValue);
            }
            catch (StorageSiteNotFoundException exception)
            {
                return HandleResourceNotFoundException(exception);
            }
            catch (Exception exception)
            {
                return HandleUnexpectedException(exception);
            }
        }

        /// <summary>
        /// Gets the history of a storage site environemental factor history.
        /// </summary>
        /// <param name="id">ID of the storage site.</param>
        /// <param name="factor">The factor to get data for.</param>
        /// <param name="startTime">The history start time.</param>
        /// <param name="endTime">The history end time.</param>
        /// <returns>Returns the history.</returns>
        private IActionResult GetStorageSiteEnvironementalFactorHistory(Guid id, EnvironmentalFactor factor, DateTime startTime, DateTime endTime)
        {
            // Validate times
            if (startTime == null || startTime == default)
            {
                return HandleBadRequest("No start time provided.");
            }
            startTime = startTime.ToUniversalTime();
            if (endTime == null || endTime == default)
            {
                endTime = DateTime.UtcNow;
            }
            if (startTime > DateTime.UtcNow || startTime > endTime)
            {
                return HandleBadRequest("Invalid start time provided.");
            }

            // Get history
            try
            {
                IEnumerable<DataPoint> history = EnvironmentService.GetHistory(id, factor, startTime, endTime);
                return Ok(history);
            }
            catch (StorageSiteNotFoundException exception)
            {
                return HandleResourceNotFoundException(exception);
            }
            catch (Exception exception)
            {
                return HandleUnexpectedException(exception);
            }
        }

        /// <summary>
        /// Gets the min and max values of a storage site environemental factor.
        /// </summary>
        /// <param name="id">ID of the storage site.</param>
        /// <param name="factor">The factor to get data for.</param>
        /// <param name="startTime">The period start time.</param>
        /// <param name="endTime">The period end time.</param>
        /// <returns>Returns the maxima.</returns>
        private IActionResult GetStorageSiteEnvironementalFactorExtrema(Guid id, EnvironmentalFactor factor, DateTime startTime, DateTime endTime)
        {
            // Validate times
            if (startTime == null || startTime == default)
            {
                return HandleBadRequest("No start time provided.");
            }
            startTime = startTime.ToUniversalTime();
            if (endTime == null || endTime == default)
            {
                endTime = DateTime.UtcNow;
            }
            if (startTime > DateTime.UtcNow || startTime > endTime)
            {
                return HandleBadRequest("Invalid start time provided.");
            }

            // Get extrema
            try
            {
                Extrema extrema = EnvironmentService.GetExtrema(id, factor, startTime, endTime);
                return Ok(extrema);
            }
            catch (StorageSiteNotFoundException exception)
            {
                return HandleResourceNotFoundException(exception);
            }
            catch (Exception exception)
            {
                return HandleUnexpectedException(exception);
            }
        }

        #endregion

        #region Administrative features

        /// <summary>
        /// Creates a new storage site.
        /// </summary>
        /// <param name="storageSiteCreationRequest">The dat needed for creating a site.</param>
        /// <returns>Returns a `201 Created` response.</returns>
        [HttpPost, Authorize(Policy = AuthPolicyNames.MAY_CREATE_STORAGE_SITE)]
        public IActionResult CreateStorageSite([FromBody] StorageSiteCreationRequest storageSiteCreationRequest)
        {
            if (storageSiteCreationRequest == null ||
                string.IsNullOrWhiteSpace(storageSiteCreationRequest.Name))
            {
                return HandleBadRequest("A valid storage site name has to be supplied.");
            }

            StorageSite site = LocationsService.CreateStorageSite(storageSiteCreationRequest.Name);
            return Created(GetNewResourceUri(site.Id), site);
        }

        /// <summary>
        /// Updates a storage site's master data (name).
        /// </summary>
        /// <param name="id">ID of the storage site to update.</param>
        /// <param name="storageSiteMasterDataUpdateRequest">Data to update.</param>
        /// <returns>Returns the updated storage site on success.</returns>
        [HttpPatch("{id}"), Authorize(Policy = AuthPolicyNames.MAY_UPDATE_STORAGE_SITE)]
        public IActionResult UpdateStorageSiteMasterData(Guid id, [FromBody] StorageSiteMasterDataUpdateRequest storageSiteMasterDataUpdateRequest)
        {
            if (id == null || storageSiteMasterDataUpdateRequest == null ||
                string.IsNullOrWhiteSpace(storageSiteMasterDataUpdateRequest.Name))
            {
                return HandleBadRequest("A valid storage site ID and name have to be supplied.");
            }

            try
            {
                StorageSite site = LocationsService.UpdateStorageSiteMasterData(id, storageSiteMasterDataUpdateRequest.Name);
                return Ok(site);
            }
            catch (StorageSiteNotFoundException exception)
            {
                return HandleResourceNotFoundException(exception);
            }
            catch (Exception exception)
            {
                return HandleUnexpectedException(exception);
            }
        }

        /// <summary>
        /// Creates a new storage area for a given storage site.
        /// </summary>
        /// <param name="siteId">ID of the site to create an area for.</param>
        /// <param name="storageAreaCreationRequest">The data for area creation.</param>
        /// <returns>Returns a `201 Created` response.</returns>
        [HttpPost("{siteId}/areas"), Authorize(Policy = AuthPolicyNames.MAY_CREATE_STORAGE_AREA)]
        public IActionResult CreateStorageArea(Guid siteId, [FromBody] StorageAreaCreationRequest storageAreaCreationRequest)
        {
            if (storageAreaCreationRequest == null ||
                string.IsNullOrWhiteSpace(storageAreaCreationRequest.Name))
            {
                return HandleBadRequest("A valid storage area name has to be supplied.");
            }

            StorageArea area = LocationsService.AddAreaToStorageSite(siteId, storageAreaCreationRequest.Name);
            return Created(GetNewResourceUri(area.Id), area);
        }
        
        /// <summary>
        /// Updates a given area of a given storage site.
        /// </summary>
        /// <param name="siteId">ID of the site for which to update an area.</param>
        /// <param name="areaId">ID of the area to update.</param>
        /// <param name="storageAreaUpdateRequest">Data to update.</param>
        /// <returns>Returns the updated area.</returns>
        [HttpPatch("{siteId}/areas/{areaId}"), Authorize(Policy = AuthPolicyNames.MAY_UPDATE_STORAGE_AREA)]
        public IActionResult UpdateStorageArea(Guid siteId, Guid areaId, [FromBody] StorageAreaUpdateRequest storageAreaUpdateRequest)
        {
            if (siteId == null || areaId == null || storageAreaUpdateRequest == null ||
                string.IsNullOrWhiteSpace(storageAreaUpdateRequest.Name))
            {
                return HandleBadRequest("A valid storage site ID, area ID and name have to be supplied.");
            }

            try
            {
                StorageArea area = LocationsService.UpdateStorageArea(siteId, areaId, storageAreaUpdateRequest.Name);
                return Ok(area);
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

        #endregion
    }
}
