namespace OpenMTS.Controllers.Contracts.Requests
{
    /// <summary>
    /// A contract for a request to create a material type.
    /// </summary>
    public class CreateMaterialTypeRequest
    {
        /// <summary>
        /// ID of the material type to create.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// The name of the material type to create.
        /// </summary>
        public string Name { get; set; }
    }
}
