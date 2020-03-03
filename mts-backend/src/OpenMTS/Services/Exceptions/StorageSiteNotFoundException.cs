using System;

namespace OpenMTS.Services.Exceptions
{
    public class StorageSiteNotFoundException : Exception, IResourceNotFoundException
    {
        /// <summary>
        /// Creates a generic storage-site-not-found exception.
        /// </summary>
        public StorageSiteNotFoundException() : base("User could not be found.") { }

        /// <summary>
        /// Creates an exception indicating what storage site ID was not found.
        /// </summary>
        /// <param name="storageSiteId">ID that was not found.</param>
        public StorageSiteNotFoundException(Guid storageSiteId) : base($"Storage site `{storageSiteId}` could not be found.") { }
    }
}
