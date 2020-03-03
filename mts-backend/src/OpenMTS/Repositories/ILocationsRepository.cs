using OpenMTS.Models;
using System;
using System.Collections.Generic;

namespace OpenMTS.Repositories
{
    /// <summary>
    /// A generic interface for a repostitory granting access to storage site and area data.
    /// </summary>
    public interface ILocationsRepository
    {
        /// <summary>
        /// Gets all available storage site.
        /// </summary>
        /// <returns>Returns all storage sites.</returns>
        IEnumerable<StorageSite> GetAllStorageSites();
        
        /// <summary>
        /// Searches storage sites using a partial name.
        /// </summary>
        /// <param name="partialName">String to search for in site names.</param>
        /// <returns>Returns filtered storage sites.</returns>
        IEnumerable<StorageSite> SearchStorageSitesByName(string partialName);

        /// <summary>
        /// Gets a storage site by it's ID.
        /// </summary>
        /// <param name="id">ID of the storage site to get.</param>
        /// <returns>Returns the storage site or null.</returns>
        StorageSite GetStorageSite(Guid id);

        /// <summary>
        /// Creates a new storage site.
        /// </summary>
        /// <param name="name">Name of the new storage site.</param>
        /// <returns>Returns the newly created storage site.</returns>
        StorageSite CreateStorageSite(string name);

        /// <summary>
        /// Updates an existing storage site.
        /// </summary>
        /// <param name="storageSite">Storage site to update.</param>
        /// <returns>Returns the updated storage site.</returns>
        StorageSite UpdateStorageSite(StorageSite storageSite);

        /// <summary>
        /// Delets a storage site.
        /// </summary>
        /// <param name="id">ID of the storage site to delete.</param>
        void DeleteStorageSite(Guid id);
    }
}
