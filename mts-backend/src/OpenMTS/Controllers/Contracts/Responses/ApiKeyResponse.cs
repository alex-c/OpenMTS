using OpenMTS.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OpenMTS.Controllers.Contracts.Responses
{
    /// <summary>
    /// Response contract for API keys.
    /// </summary>
    public class ApiKeyResponse
    {
        /// <summary>
        /// The ID of the API key.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Name of the API key.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Whether this key is enabled.
        /// </summary>
        public bool Enabled { get; set; }

        /// <summary>
        /// The atomar access rights granted to this key.
        /// </summary>
        public IEnumerable<string> Rights { get; set; }

        /// <summary>
        /// Generates an API key response contract from an API key model.
        /// </summary>
        /// <param name="key">Key to generate an ID for.</param>
        public ApiKeyResponse(ApiKey key)
        {
            Id = key.Id;
            Name = key.Name;
            Enabled = key.Enabled;
            Rights = new List<string>(key.Rights.Select(r => r.Id));
        }
    }
}
