namespace OpenMTS.Controllers.Contracts.Requests
{
    /// <summary>
    /// A request to update an API key's status.
    /// </summary>
    public class ApiKeyStatusUpdateRequest
    {
        /// <summary>
        /// Whether the key is enabled.
        /// </summary>
        public bool Enabled { get; set; }
    }
}
