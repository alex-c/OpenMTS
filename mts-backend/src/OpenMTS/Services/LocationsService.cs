using Microsoft.Extensions.Logging;
using OpenMTS.Models;
using OpenMTS.Repositories;
using OpenMTS.Services.Exceptions;
using OpenMTS.Services.Support;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OpenMTS.Services
{
    /// <summary>
    /// A service for locations: storage sites and areas.
    /// </summary>
    public class LocationsService : IPublisher<StorageSite>
    {
        /// <summary>
        /// Subscribers to notify about new storage sites.
        /// </summary>
        private List<ISubscriber<StorageSite>> Subscribers { get; }

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
            Subscribers = new List<ISubscriber<StorageSite>>();
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
            StorageSite site = LocationsRepository.CreateStorageSite(name);
            Publish(site); // Notify environment data service of new site
            return site;
        }

        /// <summary>
        /// Updates a storage site.
        /// </summary>
        /// <param name="site">Site to update.</param>
        /// <returns>Returns updated site.</returns>
        public StorageSite UpdateStorageSiteMasterData(Guid id, string name)
        {
            StorageSite site = GetStorageSiteOrThrowNotFoundException(id);
            site.Name = name;
            return LocationsRepository.UpdateStorageSite(site);
        }

        /// <summary>
        /// Creates a new storage area for a given storage site.
        /// </summary>
        /// <param name="siteId">ID of the site to create an area for.</param>
        /// <param name="areaName">Name of the area to create.</param>
        /// <returns>Returns the newly created area.</returns>
        public StorageArea AddAreaToStorageSite(Guid siteId, string areaName)
        {
            StorageSite site = GetStorageSiteOrThrowNotFoundException(siteId);
            return LocationsRepository.CreateStorageArea(site, areaName);
        }

        /// <summary>
        /// Updates a given area of a given storage site.
        /// </summary>
        /// <param name="siteId">ID of the site for which to update an area.</param>
        /// <param name="areaId">ID of the area to update.</param>
        /// <param name="areaName">Area name to update.</param>
        /// <returns>Returns the updated area.</returns>
        public StorageArea UpdateStorageArea(Guid siteId, Guid areaId, string areaName)
        {
            StorageSite site = GetStorageSiteOrThrowNotFoundException(siteId);
            StorageArea area = site.Areas.Where(a => a.Id == areaId).FirstOrDefault();
            if (area == null)
            {
                throw new StorageAreaNotFoundException(siteId, areaId);
            }
            area.Name = areaName;
            area = LocationsRepository.UpdateStorageArea(area);
            return area;
        }

        #region IPublisher implementation

        /// <summary>
        /// Subscribes the specified subscriber.
        /// </summary>
        /// <param name="subscriber">The subscriber.</param>
        public void Subscribe(ISubscriber<StorageSite> subscriber)
        {
            if (!Subscribers.Contains(subscriber))
            {
                Subscribers.Add(subscriber);
            }
        }

        /// <summary>
        /// Unsubscribes the specified subscriber.
        /// </summary>
        /// <param name="subscriber">The subscriber.</param>
        public void Unsubscribe(ISubscriber<StorageSite> subscriber)
        {
            Subscribers.Remove(subscriber);
        }

        /// <summary>
        /// Publishes the specified site.
        /// </summary>
        /// <param name="site">The site.</param>
        public void Publish(StorageSite site)
        {
            foreach (ISubscriber<StorageSite> subscriber in Subscribers)
            {
                subscriber.OnPublish(site);
            }
        }

        #endregion

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
