using Microsoft.AspNetCore.Authorization;
using OpenMTS.Models;

namespace OpenMTS.Authorization
{
    /// <summary>
    /// A generic requirement that says any one of a given role and a given right suffice for authorization.
    /// </summary>
    public class AccessRightsRequirement : IAuthorizationRequirement
    {
        /// <summary>
        /// The role that grants authorization.
        /// </summary>
        public Role Role { get; }

        /// <summary>
        /// The right that grants authorization.
        /// </summary>
        public string RightId { get; }

        /// <summary>
        /// Creates a requirement for a given role and/or right.
        /// </summary>
        /// <param name="role">A role or null.</param>
        /// <param name="rightId">A right ID or null.</param>
        public AccessRightsRequirement(Role role, string rightId)
        {
            Role = role;
            RightId = rightId;
        }
    }
}
