namespace OpenMTS.Controllers.Contracts.Requests
{
    /// <summary>
    /// A contract for a request to update a custom material prop.
    /// </summary>
    public class CustomMaterialPropUpdateRequest
    {
        /// <summary>
        /// The prop name to set.
        /// </summary>
        public string Name { get; set; }
    }
}
