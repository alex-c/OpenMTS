using Newtonsoft.Json;

namespace OpenMTS.Services.Authentication.Providers.UserLogin
{
    /// <summary>
    /// The data model for authentication requests of the user login type.
    /// </summary>
    public class UserLoginData
    {
        /// <summary>
        /// The unique ID of the user to log in.
        /// </summary>
        [JsonProperty(Required = Required.Always)]
        public string Id { get; set; }

        /// <summary>
        /// The password of the user to log in.
        /// </summary>
        [JsonProperty(Required=Required.Always)]
        public string Password { get; set; }
    }
}
