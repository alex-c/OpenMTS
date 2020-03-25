using OpenMTS.Models;
using System;
using System.Collections.Generic;

namespace OpenMTS.Repositories
{
    /// <summary>
    /// Generic interface for a read-only API key repository.
    /// </summary>
    public interface IReadOnlyApiKeyProvider
    {
        /// <summary>
        /// Gets all available API keys.
        /// </summary>
        /// <returns>Returns all API keys.</returns>
        IEnumerable<ApiKey> GetAllApiKeys();

        /// <summary>
        /// Gets an API key by it's ID.
        /// </summary>
        /// <param name="id">ID of the key to get.</param>
        /// <returns>Returns the API key if found, else null.</returns>
        ApiKey GetApiKey(Guid id);
    }
}
