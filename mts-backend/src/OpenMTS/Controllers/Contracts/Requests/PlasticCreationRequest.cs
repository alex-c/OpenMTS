namespace OpenMTS.Controllers.Contracts.Requests
{
    /// <summary>
    /// A contract for a request to create a plastic.
    /// </summary>
    public class PlasticCreationRequest
    {
        /// <summary>
        /// ID of the plastic to create.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// The name of the plastic to create.
        /// </summary>
        public string Name { get; set; }
    }
}
