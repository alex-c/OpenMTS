namespace OpenMTS.Controllers.Contracts.Requests
{

    /// <summary>
    /// A contract for a request to update a custom material batch property.
    /// </summary>
    public class CustomBatchPropUpdateRequest
    {
        /// <summary>
        /// Name of the property to set.
        /// </summary>
        public string Name { get; set; }
    }
}
