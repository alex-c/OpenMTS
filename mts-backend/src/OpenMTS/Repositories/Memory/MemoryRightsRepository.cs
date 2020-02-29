using System;
using System.Collections.Generic;
using System.Text;
using OpenMTS.Models;

namespace OpenMTS.Repositories.Memory
{
    /// <summary>
    /// An in-memory repository of access rights, populated on creation.
    /// </summary>
    public class MemoryRightsRepository : IRightsRepository
    {
        /// <summary>
        /// The IDs of all access rights. This is currently the single source of access rights.
        /// </summary>
        private static readonly List<string> RightIds = new List<string>()
        {
            // TODO: Define and add access rights....
            "users.create",
            "users.update",
            "users.updateStatus"
        };

        /// <summary>
        /// A mapping of access right IDs to model class instances.
        /// </summary>
        private Dictionary<string, Right> Rights { get; }

        /// <summary>
        /// Creates and populates the repository.
        /// </summary>
        public MemoryRightsRepository()
        {
            Rights = new Dictionary<string, Right>();
            foreach (string rightId in RightIds)
            {
                Rights.Add(rightId, new Right(rightId));
            }
        }

        /// <summary>
        /// Gets all existing access rights.
        /// </summary>
        /// <returns>Returns all access rights.</returns>
        public IEnumerable<Right> GetAllRights()
        {
            return Rights.Values;
        }

        /// <summary>
        /// Gets an access right by it's ID.
        /// </summary>
        /// <param name="id">ID of the access right to get.</param>
        /// <returns>Returns the access right or null if no matching right could be found.</returns>
        public Right GetRight(string id)
        {
            if (Rights.TryGetValue(id, out Right right))
            {
                return right;
            }
            return null;
        }
    }
}
