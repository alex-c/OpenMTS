namespace OpenMTS.Controllers.Contracts.Requests
{
    /// <summary>
    /// Updates a user's status.
    /// </summary>
    public class UpdateUserStatusRequest
    {
        /// <summary>
        /// Whether the user is disabled.
        /// </summary>
        public bool Disabled { get; set; }
    }
}
