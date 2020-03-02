using System;
using System.Collections.Generic;

namespace OpenMTS.Models
{
    /// <summary>
    /// An API key which allows access to the OpenMTS API with a set of atomar access rights.
    /// </summary>
    public class ApiKey
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
        public IEnumerable<Right> Rights { get; set; }
    }
}
