using OpenMTS.Models;
using System.Collections.Generic;

namespace OpenMTS.Repositories
{
    /// <summary>
    /// A read-only user repository that allows to get user data.
    /// </summary>
    public interface IReadOnlyUserRepository
    {
        /// <summary>
        /// Gets all users.
        /// </summary>
        /// <returns>Returns all users.</returns>
        IEnumerable<User> GetAllUsers();

        /// <summary>
        /// Gets a user by his unique ID.
        /// </summary>
        /// <param name="id">ID of the user.</param>
        /// <returns>Returns the user, or null if no matching user was found.</returns>
        User GetUser(string id);
    }
}
