namespace OpenMTS.Controllers.Contracts.Requests
{
    /// <summary>
    /// A contract for a request to update a plastic.
    /// </summary>
    public class PlasticUpdateRequest
    {
        /// <summary>
        /// The new name to set.
        /// </summary>
        public string Name { get; set; }
    }
}
