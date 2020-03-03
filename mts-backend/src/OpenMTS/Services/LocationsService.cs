using Microsoft.Extensions.Logging;
using OpenMTS.Models;
using OpenMTS.Repositories;
using OpenMTS.Services.Exceptions;
using System;
using System.Collections.Generic;

namespace OpenMTS.Services
{
    /// <summary>
    /// A service for locations: storage sites and areas.
    /// </summary>
    public class LocationsService
    {
        /// <summary>
        /// The underlying repository granting access to locations data.
        /// </summary>
        private ILocationsRepository LocationsRepository { get; }

        /// <summary>
        /// A logger for local logging needs.
        /// </summary>
        private ILogger Logger { get; }

        /// <summary>
        /// Sets up the service with all necessary components.
        /// </summary>
        /// <param name="loggerFactory">A factory to create loggers from.</param>
        /// <param name="locationsRepository">A repository to get locations data from.</param>
        public LocationsService(ILoggerFactory loggerFactory, ILocationsRepository locationsRepository)
        {
            Logger = loggerFactory.CreateLogger<LocationsService>();
            LocationsRepository = locationsRepository;
        }

        /// <summary>
        /// Gets all storage sites.
        /// </summary>
        /// <returns>Returns a list of storage sites.</returns>
        public IEnumerable<StorageSite> GetAllStorageSites()
        {
            return LocationsRepository.GetAllStorageSites();
        }

        /// <summary>
        /// Searches storage sites using a partial name.
        /// </summary>
        /// <param name="partialName">String to search for in site names.</param>
        /// <returns>Returns filtered storage sites.</returns>
        public IEnumerable<StorageSite> SearchStorageSitesByName(string partialName)
        {
            return LocationsRepository.SearchStorageSitesByName(partialName);
        }

        /// <summary>
        /// Gets a storage site by it's unique ID.
        /// </summary>
        /// <param name="id">ID of the storage site to get.</param>
        /// <returns>Returns the storage site or null.</returns>
        public StorageSite GetStorageSite(Guid id)
        {
            return GetStorageSiteOrThrowNotFoundException(id);
        }

        /// <summary>
        /// Creates a new storage site.
        /// </summary>
        /// <param name="name">Name of the storage site to create.</param>
        /// <returns>Returns the newly created site.</returns>
        public StorageSite CreateStorageSite(string name)
        {
            return LocationsRepository.CreateStorageSite(name);
        }

        #region Private Helpers

        /// <summary>
        /// Attempts to get a storage site from the underlying repository and throws a <see cref="StorageSiteNotFoundException"/> if no matching site could be found.
        /// </summary>
        /// <param name="id">ID of the storage site to get.</param>
        /// <exception cref="StorageSiteNotFoundException">Thrown if no matching site could be found.</exception>
        /// <returns>Returns the storage site, if found.</returns>
        private StorageSite GetStorageSiteOrThrowNotFoundException(Guid id)
        {
            StorageSite site = LocationsRepository.GetStorageSite(id);

            // Check for site existence
            if (site == null)
            {
                throw new StorageSiteNotFoundException(id);
            }

            return site;
        }

        #endregion
    }
}
