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
        /// <param name="showArchived">Whether to return soft-deleted users.</param>
        /// <returns>Returns all users.</returns>
        IEnumerable<User> GetAllUsers(bool showArchived = false);

        /// <summary>
        /// Searches users by name using a partial name.
        /// </summary>
        /// <param name="partialName">Partial name to search for.</param>
        /// <param name="showArchived">Whether to return soft-deleted users.</param>
        /// <returns>Returns a list of matching users.</returns>
        IEnumerable<User> SearchUsersByName(string partialName, bool showArchived);

        /// <summary>
        /// Gets a user by his unique ID.
        /// </summary>
        /// <param name="id">ID of the user.</param>
        /// <returns>Returns the user, or null if no matching user was found.</returns>
        User GetUser(string id);
    }
}
