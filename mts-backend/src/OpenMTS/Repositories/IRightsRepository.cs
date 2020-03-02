using OpenMTS.Models;
using System.Collections.Generic;

namespace OpenMTS.Repositories
{
    /// <summary>
    /// A generic interface for a repository of OpenMTS access rights.
    /// </summary>
    public interface IRightsRepository
    {
        /// <summary>
        /// Gets all existing rights.
        /// </summary>
        /// <returns>Returns all rights.</returns>
        IEnumerable<Right> GetAllRights();

        /// <summary>
        /// Gets an access right by it's ID, or null if no matching right was found.
        /// </summary>
        /// <param name="id">ID of the access right to get.</param>
        /// <returns>Returns the access right or null if no matching right was found.</returns>
        Right GetRight(string id);
    }
}
