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
        /// Injects all needed components.
        /// </summary>
        /// <param name="loggerFactory">A factory to create loggers from.</param>
        /// <param name="locationsService">A service for locations-related functionality.</param>
        public LocationsController(ILoggerFactory loggerFactory, LocationsService locationsService)
        {
            Logger = loggerFactory.CreateLogger<LocationsController>();
            LocationsService = locationsService;
        }

        #region Public getters

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
