using OpenMTS.Models;
using System.Collections.Generic;

namespace OpenMTS.Repositories
{
    /// <summary>
    /// A generic interface for a repository of plastics (material types).
    /// </summary>
    public interface IPlasticsRepository
    {
        /// <summary>
        /// Gets all available plastics.
        /// </summary>
        /// <returns>Returns all plastics.</returns>
        IEnumerable<Plastic> GetAllPlastics();

        /// <summary>
        /// Gets and filters plastics using a partial name.
        /// </summary>
        /// <param name="partialName">String to filter with..</param>
        /// <returns>Returns filtered plastics.</returns>
        IEnumerable<Plastic> SearchPlasticsByName(string partialName);

        /// <summary>
        /// Gets a plastic.
        /// </summary>
        /// <param name="id">The ID of the plastic to get.</param>
        /// <returns>Returns the plastic or null.</returns>
        Plastic GetPlastic(string id);

        /// <summary>
        /// Creates a new plastic.
        /// </summary>
        /// <param name="id">The ID of the new plastic.</param>
        /// <param name="name">The name of the plastic to create.</param>
        /// <returns>Returns the newly created plastic.</returns>
        Plastic CreatePlastic(string id, string name);

        /// <summary>
        /// Updates a plastic.
        /// </summary>
        /// <param name="plastic">The plastic to update.</param>
        void UpdatePlastic(Plastic plastic);

        /// <summary>
        /// Deletes a plastic.
        /// </summary>
        /// <param name="id">The ID of the plastic to delete.</param>
        void DeletePlastic(string id);
    }
}
