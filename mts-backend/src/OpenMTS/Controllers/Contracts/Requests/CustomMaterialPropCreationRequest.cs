using OpenMTS.Models;

namespace OpenMTS.Controllers.Contracts.Requests
{
    /// <summary>
    /// Contract for a request to create a custom material property.
    /// </summary>
    public class CustomMaterialPropCreationRequest
    {
        /// <summary>
        /// Name of the prop to create.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Type of the prop to create.
        /// </summary>
        public int Type { get; set; }
    }
}
