using OpenMTS.Models;
using System.Collections.Generic;

namespace OpenMTS.Services.Authentication
{
    /// <summary>
    /// Describes the required functionality of an authentication provider.
    /// </summary>
    public interface IAuthenticationProvider
    {
        /// <summary>
        /// The authentication method provided by the implementation.
        /// </summary>
        AuthenticationMethod AuthenticationMethod { get; }

        /// <summary>
        /// Attempts to authenticate a client.
        /// </summary>
        /// <param name="data">The data needed for authentication.</param>
        /// <param name="subject">The subject that was authenticated.</param>
        /// <param name="roles">The roles of the authenticated subject.</param>
        /// <param name="rights">The rights of the authenticated subject.</param>
        /// <returns>Returns whether the client was successfully authenticated.</returns>
        bool TryAuthenticate(string data, out string subject, out IEnumerable<Role> roles, out IEnumerable<Right> rights);
    }
}
