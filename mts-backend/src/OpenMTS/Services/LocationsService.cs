using Microsoft.Extensions.Logging;
using OpenMTS.Models;
using OpenMTS.Repositories;
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
        /// Creates a new storage site.
        /// </summary>
        /// <param name="name">Name of the storage site to create.</param>
        /// <returns>Returns the newly created site.</returns>
        public StorageSite CreateStorageSite(string name)
        {
            return LocationsRepository.CreateStorageSite(name);
        }
    }
}
