using OpenMTS.Models;
using System;
using System.Collections.Generic;

namespace OpenMTS.Repositories
{
    /// <summary>
    /// Generic interface for an API key repository.
    /// </summary>
    public interface IApiKeyRepository
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

        /// <summary>
        /// Creates a new API key.
        /// </summary>
        /// <param name="name">Name of the API key to create.</param>
        /// <returns>Returns the newly crated API key.</returns>
        ApiKey CreateApiKey(string name);

        /// <summary>
        /// Updates an existing API key.
        /// </summary>
        /// <param name="apiKey">key to update.</param>
        void UpdateApiKey(ApiKey apiKey);

        /// <summary>
        /// Deletes an API key.
        /// </summary>
        /// <param name="id">ID of the key to delete.</param>
        void DeleteApiKey(Guid id);
    }
}
