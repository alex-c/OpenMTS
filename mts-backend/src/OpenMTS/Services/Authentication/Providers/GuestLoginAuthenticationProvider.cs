using OpenMTS.Models;
using System.Collections.Generic;

namespace OpenMTS.Services.Authentication.Providers
{
    /// <summary>
    /// Provides authentication for a password-less guest login.
    /// </summary>
    public class GuestLoginAuthenticationProvider : AuthenticationProviderBase, IAuthenticationProvider
    {
        /// <summary>
        /// Indicates which authentication method this provider is for.
        /// </summary>
        public AuthenticationMethod AuthenticationMethod { get; } = AuthenticationMethod.GuestLogin;

        /// <summary>
        /// Provides a token for the guest login authentication. This does not perform any authentication checks,
        /// it just delivers a token witht he `User` role.
        /// </summary>
        /// <param name="data">Not used in this provider.</param>
        /// <param name="subject">The `openmts.guest` subject.</param>
        /// <param name="roles">Contains the `User` role.</param>
        /// <param name="rights">Contains no special rights..</param>
        /// <returns>Always returns true..</returns>
        public bool TryAuthenticate(string data, out string subject, out IEnumerable<Role> roles, out IEnumerable<Right> rights)
        {
            // TODO: move subject string to constant
            subject = "openmts.guest";
            roles = new List<Role>() { Role.User };
            rights = new List<Right>();
            return true;
        }
    }
}
