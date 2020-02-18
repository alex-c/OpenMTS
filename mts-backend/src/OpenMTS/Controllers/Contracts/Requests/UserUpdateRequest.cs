namespace OpenMTS.Controllers.Contracts.Requests
{
    /// <summary>
    /// A request to update an user.
    /// </summary>
    public class UserUpdateRequest
    {
        /// <summary>
        /// A user name to set.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The user role to assign to the user.
        /// </summary>
        public int Role { get; set; }
    }
}
