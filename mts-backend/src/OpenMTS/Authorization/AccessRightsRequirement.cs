using Microsoft.AspNetCore.Authorization;
using OpenMTS.Models;
using System.Collections.Generic;

namespace OpenMTS.Authorization
{
    /// <summary>
    /// A generic requirement that says any one of a given role and a given right suffice for authorization.
    /// </summary>
    public class AccessRightsRequirement : IAuthorizationRequirement
    {
        /// <summary>
        /// The roles that grant authorization.
        /// </summary>
        public IEnumerable<Role> Roles { get; }

        /// <summary>
        /// The right that grants authorization.
        /// </summary>
        public string RightId { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="AccessRightsRequirement"/> class.
        /// </summary>
        public AccessRightsRequirement()
        {
            Roles = new List<Role>();
            RightId = null;
        }

        /// <summary>
        /// Creates a requirement for a given list of roles role and/or a right.
        /// </summary>
        /// <param name="roles">Roles that grant authorization.</param>
        /// <param name="rightId">A right ID or null.</param>
        public AccessRightsRequirement(IEnumerable<Role> roles, string rightId)
        {
            Roles = roles;
            RightId = rightId;
        }
    }
}
