using Newtonsoft.Json;
using System;

namespace OpenMTS.Services.Authentication.Providers.ApiKeys
{
    /// <summary>
    /// The data model for authentication requests using an API key.
    /// </summary>
    public class ApiKeyData
    {
        /// <summary>
        /// The API key to authenticate wiht.
        /// </summary>
        [JsonProperty(Required = Required.Always)]
        public Guid ApiKey { get; set; }
    }
}
