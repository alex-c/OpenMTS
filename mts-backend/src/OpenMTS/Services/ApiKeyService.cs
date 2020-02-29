using Microsoft.Extensions.Logging;
using OpenMTS.Models;
using OpenMTS.Repositories;
using OpenMTS.Services.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace OpenMTS.Services
{
    /// <summary>
    /// A service for the management of API keys.
    /// </summary>
    public class ApiKeyService
    {
        /// <summary>
        /// The underlying repository containing API keys.
        /// </summary>
        private IApiKeyRepository ApiKeyRepository { get; }

        /// <summary>
        /// A logger for local logging purposes.
        /// </summary>
        private ILogger Logger { get; }

        /// <summary>
        /// Sets up the service with all needed component.
        /// </summary>
        /// <param name="loggerFactory">A factory to create loggers from.</param>
        /// <param name="apiKeyRepository">Grants access to API keys.</param>
        public ApiKeyService(ILoggerFactory loggerFactory, IApiKeyRepository apiKeyRepository)
        {
            ApiKeyRepository = apiKeyRepository;
            Logger = loggerFactory.CreateLogger<ApiKeyService>();
        }

        /// <summary>
        /// Gets all available API keys, whether they are enabled or not.
        /// </summary>
        /// <returns>Returns all API keys.</returns>
        public IEnumerable<ApiKey> GetAllApiKeys()
        {
            return ApiKeyRepository.GetAllApiKeys();
        }

        /// <summary>
        /// Gets an API key by it's ID.
        /// </summary>
        /// <param name="id">ID of the API key to get.</param>
        /// <returns>Returns the API key if found.</returns>
        /// <exception cref="ApiKeyNotFoundException">Thrown if no matching API key could be found.</exception>
        public ApiKey GetApiKey(Guid id)
        {
            return GetApiKeyOrThrowNotFoundException(id);
        }

        /// <summary>
        /// Creates a new API key. API keys have no rights and are disabled upon creation.
        /// </summary>
        /// <param name="name">Name of the API key to create.</param>
        /// <returns>Returns the newly created key.</returns>
        public ApiKey CreateApiKey(string name)
        {
            return ApiKeyRepository.CreateApiKey(name);
        }

        /// <summary>
        /// Updates an API key, which allows to change it's name and set the rights granted to this key.
        /// </summary>
        /// <param name="id">ID of the key to update.</param>
        /// <param name="name">New name of the key.</param>
        /// <param name="rights">Rights to grant to the key.</param>
        /// <returns>Returns the updated key.</returns>
        /// <exception cref="ApiKeyNotFoundException">Thrown if no matching API key could be found.</exception>
        public ApiKey UpdateApiKey(Guid id, string name, IEnumerable<Right> rights)
        {
            ApiKey key = GetApiKeyOrThrowNotFoundException(id);

            key.Name = name;
            key.Rights = rights;
            ApiKeyRepository.UpdateApiKey(key);

            return key;
        }

        /// <summary>
        /// Allows to enable or disable an API key.
        /// </summary>
        /// <param name="id">ID of the key to update the status for.</param>
        /// <param name="isEnabled">Whether the key is supposed to be enabled.</param>
        /// <exception cref="ApiKeyNotFoundException">Thrown if no matching API key could be found.</exception>
        public void UpdateApiKeyStatus(Guid id, bool isEnabled)
        {
            ApiKey key = GetApiKeyOrThrowNotFoundException(id);

            key.Enabled = isEnabled;
            ApiKeyRepository.UpdateApiKey(key);
        }

        /// <summary>
        /// Deletes an API key.
        /// </summary>
        /// <param name="id">ID of the key to delete.</param>
        public void DeleteApiKey(Guid id)
        {
            ApiKeyRepository.DeleteApiKey(id);
        }

        #region Private Helpers

        /// <summary>
        /// Attempts to get an API key from the underlying repository and throws a <see cref="ApiKeyNotFoundException"/> if no matching key could be found.
        /// </summary>
        /// <param name="id">ID of the key to get.</param>
        /// <exception cref="ApiKeyNotFoundException">Thrown if no matching key could be found.</exception>
        /// <returns>Returns the key, if found.</returns>
        private ApiKey GetApiKeyOrThrowNotFoundException(Guid id)
        {
            ApiKey key = ApiKeyRepository.GetApiKey(id);

            // Check for key existence
            if (key == null)
            {
                throw new ApiKeyNotFoundException(id);
            }

            return key;
        }

        #endregion
    }
}
