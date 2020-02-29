using System;
using System.Collections.Generic;
using System.Text;

namespace OpenMTS.Controllers.Contracts.Requests
{
    /// <summary>
    /// A contract for a request to update an existing API key.
    /// </summary>
    public class ApiKeyUpdateRequest
    {
        /// <summary>
        /// The new name of the key.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The rights to grant to this key.
        /// </summary>
        public IEnumerable<string> Rights { get; set; }
    }
}
