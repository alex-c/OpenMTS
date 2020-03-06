namespace OpenMTS.Controllers.Contracts.Requests
{
    /// <summary>
    /// A contract for a request to create a new material.
    /// </summary>
    public class MaterialCreationRequest
    {
        /// <summary>
        /// Name of the new material.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The manufacturer of the new material.
        /// </summary>
        public string Manufacturer { get; set; }

        /// <summary>
        /// The manufacturer's ID for this material.
        /// </summary>
        public string ManufacturerId { get; set; }

        /// <summary>
        /// The type of this material as a string.
        /// </summary>
        public string Type { get; set; }
    }
}
