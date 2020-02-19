namespace OpenMTS.Models
{
    /// <summary>
    /// Represents the OpenMTS configuration options available to Administrators.
    /// </summary>
    public class Configuration
    {
        /// <summary>
        /// Whether to allow a password-less guest login.
        /// </summary>
        public bool AllowGuestLogin { get; set; }
    }
}
