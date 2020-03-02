namespace OpenMTS.Controllers.Contracts.Requests
{
    /// <summary>
    /// A contract for API key creation requests.
    /// </summary>
    public class ApiKeyCreationRequest
    {
        /// <summary>
        /// Name of the API key to create.
        /// </summary>
        public string Name { get; set; }
    }
}
