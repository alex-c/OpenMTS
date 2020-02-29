using System;

namespace OpenMTS.Services.Exceptions
{
    /// <summary>
    /// Thrown when an API key was not found.
    /// </summary>
    public class ApiKeyNotFoundException : Exception, IResourceNotFoundException
    {
        /// <summary>
        /// Creates a generic key-not-found exception.
        /// </summary>
        public ApiKeyNotFoundException() : base("API key could not be found.") { }

        /// <summary>
        /// Creates an exception telling what API key ID was not found.
        /// </summary>
        /// <param name="keyId">API key ID that was not found.</param>
        public ApiKeyNotFoundException(Guid keyId) : base($"API key `{keyId}` could not be found.") { }
    }
}
