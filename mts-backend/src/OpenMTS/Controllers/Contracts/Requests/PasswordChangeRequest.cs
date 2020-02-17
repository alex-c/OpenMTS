namespace OpenMTS.Controllers.Contracts.Requests
{
    /// <summary>
    /// A request to change a user's password.
    /// </summary>
    public class PasswordChangeRequest
    {
        /// <summary>
        /// The user's old password.
        /// </summary>
        public string Old { get; set; }

        /// <summary>
        /// A new user password.
        /// </summary>
        public string New { get; set; }
    }
}
