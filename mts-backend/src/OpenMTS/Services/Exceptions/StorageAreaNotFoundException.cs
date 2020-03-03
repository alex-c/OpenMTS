using System;

namespace OpenMTS.Services.Exceptions
{
    /// <summary>
    /// Inidicates that a storage area could not be found.
    /// </summary>
    class StorageAreaNotFoundException : Exception, IResourceNotFoundException
    {
        /// <summary>
        /// Creates a generic storage-area-not-found exception.
        /// </summary>
        public StorageAreaNotFoundException() : base("Storage area could not be found.") { }

        /// <summary>
        /// Creates an exception indicating what storage area ID was not found.
        /// </summary>
        /// <param name="storageSiteId">ID of the storage site checked.</param>
        /// <param name="storageAreaId">ID of the area that was not found.</param>
        public StorageAreaNotFoundException(Guid storageSiteId, Guid storageAreaId) : base($"Storage area `{storageAreaId}` could not be found for storage site `{storageSiteId}`.") { }
    }
}
