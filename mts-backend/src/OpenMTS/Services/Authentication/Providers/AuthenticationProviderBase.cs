using Newtonsoft.Json;

namespace OpenMTS.Services.Authentication.Providers
{
    /// <summary>
    /// Base functionality that is or might be shared across several authentication providers.
    /// </summary>
    public class AuthenticationProviderBase
    {
        /// <summary>
        /// Parses JSON data needed for authentication and converts it into a target model.
        /// </summary>
        /// <typeparam name="TAuthData">The data model to convert to.</typeparam>
        /// <param name="data">The data to parse.</param>
        /// <returns>Returns whether parsing was successful.</returns>
        protected TAuthData ParseData<TAuthData>(string data)
        {
            try
            {
                return JsonConvert.DeserializeObject<TAuthData>(data);
            }
            catch (JsonSerializationException)
            {
                throw new MalformedAuthenticationDataException();
            }
        }
    }
}
