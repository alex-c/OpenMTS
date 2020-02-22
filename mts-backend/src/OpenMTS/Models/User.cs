namespace OpenMTS.Models
{
    /// <summary>
    /// A user of the OpenMTS platform.
    /// </summary>
    public class User
    {
        /// <summary>
        /// The user's unique login ID.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// The user's name as it will be displayed in the UI.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The user's hashed password.
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// The salt used to hash this user's password.
        /// </summary>
        public byte[] Salt { get; set; }

        /// <summary>
        /// The platform role the user has been assigned.
        /// </summary>
        public Role Role { get; set; }

        /// <summary>
        /// Whether the user is disabled (soft delete).
        /// </summary>
        public bool Disabled { get; set; }
    }
}
