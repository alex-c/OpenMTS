using Microsoft.Extensions.Logging;
using OpenMTS.Models;
using OpenMTS.Repositories;
using System.Collections.Generic;

namespace OpenMTS.Services
{
    /// <summary>
    /// Provides functionality for access rights.
    /// </summary>
    public class RightsService
    {
        /// <summary>
        /// The underlying repository for access rights.
        /// </summary>
        private IRightsRepository RightsRepository { get; }

        /// <summary>
        /// A logger for local logging needs.
        /// </summary>
        private ILogger Logger { get; }

        /// <summary>
        /// Creates a service instance with all needed components.
        /// </summary>
        /// <param name="loggerFactory">The factory to create a logger from.</param>
        /// <param name="rightsRepository">The repository for access rights.</param>
        public RightsService(ILoggerFactory loggerFactory, IRightsRepository rightsRepository)
        {
            RightsRepository = rightsRepository;
            Logger = loggerFactory.CreateLogger<RightsService>();
        }

        /// <summary>
        /// Gets all existing access rights.
        /// </summary>
        /// <returns>Returns all access rights.</returns>
        public IEnumerable<Right> GetAllRights()
        {
            return RightsRepository.GetAllRights();
        }

        /// <summary>
        /// Gets an access right by it's ID or null if no matching right could be found.
        /// </summary>
        /// <param name="id">ID of the access right to get.</param>
        /// <returns>Returns the access right or null if no matching right could be found.</returns>
        public Right GetRight(string id)
        {
            return RightsRepository.GetRight(id);
        }
    }
}
