namespace OpenMTS.Controllers.Contracts.Requests
{
    /// <summary>
    /// A contract for requests to update a material.
    /// </summary>
    public class MaterialUpdateRequest
    {
        /// <summary>
        /// Name of the material.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The manufacturer of the material.
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
