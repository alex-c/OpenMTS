using OpenMTS.Models;
using OpenMTS.Repositories;
using OpenMTS.Services.Authentication.Providers.ApiKeys;
using System.Collections.Generic;

namespace OpenMTS.Services.Authentication.Providers
{
    /// <summary>
    /// Provides authentication for IoT applications using pre-configured API keys.
    /// </summary>
    public class ApiKeyAuthenticationProvider : AuthenticationProviderBase, IAuthenticationProvider
    {
        /// <summary>
        /// Grants access to API key data.
        /// </summary>
        private IReadOnlyApiKeyRepository ApiKeyRepository { get; }

        /// <summary>
        /// The authentication method this provider is for.
        /// </summary>
        public AuthenticationMethod AuthenticationMethod { get; } = AuthenticationMethod.ApiKey;

        /// <summary>
        /// Sets up the provider with the underlying repositoy.
        /// </summary>
        /// <param name="apiKeyRepository">Repository for API keys.</param>
        public ApiKeyAuthenticationProvider(IReadOnlyApiKeyRepository apiKeyRepository)
        {
            ApiKeyRepository = apiKeyRepository;
        }

        /// <summary>
        /// Attempts to authenticate a IoT application using an API key.
        /// </summary>
        /// <param name="data">The <see cref="ApiKeyData"/> needed to authentication as a JSON string.</param>
        /// <param name="subject">The subject, which is the API key ID.</param>
        /// <param name="roles">No roles will be set using this authentication method.</param>
        /// <param name="rights">The rights associated to the API key.</param>
        /// <returns>Returns whether authentication was successful.</returns>
        /// <exception cref="MalformedAuthenticationDataException">Thrown if the passed data doesn't match the expected model.</exception>
        public bool TryAuthenticate(string data, out string subject, out IEnumerable<Role> roles, out IEnumerable<Right> rights)
        {
            ApiKeyData apiKeyData = ParseData<ApiKeyData>(data);

            // Initialize out-parameters
            subject = null;
            roles = new List<Role>();
            rights = null;

            // Look up key
            ApiKey key = ApiKeyRepository.GetApiKey(apiKeyData.ApiKey);
            if (key == null)
            {
                return false;
            }
            subject = key.Id.ToString();
            rights = key.Rights;

            // Check for status
            if (!key.Enabled)
            {
                return false;
            }

            // All checks passed!
            return true;
        }
    }
}
