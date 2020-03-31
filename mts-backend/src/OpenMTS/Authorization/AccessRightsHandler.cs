using Microsoft.AspNetCore.Authorization;
using OpenMTS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace OpenMTS.Authorization
{
    /// <summary>
    /// A generic authorization handler that authorizes if any of a given role and a given right are matched.
    /// </summary>
    public class AccessRightsHandler : AuthorizationHandler<AccessRightsRequirement>
    {
        /// <summary>
        /// Authorizes if any of a given role and a given right are present.
        /// </summary>
        /// <param name="context">Handler context containing the JWT.</param>
        /// <param name="requirement">Role and right requirement.</param>
        /// <returns></returns>
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, AccessRightsRequirement requirement)
        {
            Claim role = context.User.Claims.Where(c => c.Type == ClaimTypes.Role).FirstOrDefault();
            IEnumerable<Claim> rights = context.User.Claims.Where(c => c.Type == "rights");

            if (role != null && requirement.Roles.Contains(Enum.Parse<Role>(role.Value)))
            {
                context.Succeed(requirement);
            }
            else if (rights != null && rights.Select(r => r.Value).Contains(requirement.RightId))
            {
                context.Succeed(requirement);
            }
            return Task.CompletedTask;
        }
    }
}
