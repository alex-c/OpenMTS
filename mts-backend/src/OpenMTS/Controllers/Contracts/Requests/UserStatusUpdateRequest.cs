namespace OpenMTS.Controllers.Contracts.Requests
{
    /// <summary>
    /// A request to update a user's status.
    /// </summary>
    public class UserStatusUpdateRequest
    {
        /// <summary>
        /// Whether the user is disabled.
        /// </summary>
        public bool Disabled { get; set; }
    }
}
