namespace OpenMTS.Services.Authentication
{
    /// <summary>
    /// An enumeration of all supported authentication methods.
    /// </summary>
    public enum AuthenticationMethod
    {
        /// <summary>
        /// Determines that the used authentication method is a user login.
        /// </summary>
        UserLogin,

        /// <summary>
        /// Determines that the used authentication method is a guest login.
        /// </summary>
        GuestLogin,

        /// <summary>
        /// Determines that the used authentication method is a pre-configured API key.
        /// </summary>
        ApiKey
    }
}
