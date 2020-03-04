using OpenMTS.Models;
using System.Collections.Generic;

namespace OpenMTS.Repositories
{
    /// <summary>
    /// A generic interface for materials-data persistence.
    /// </summary>
    public interface IMaterialsRepository
    {
        /// <summary>
        /// Gets all materials.
        /// </summary>
        /// <returns>Returns all materials</returns>
        IEnumerable<Material> GetAllMaterials();

        /// <summary>
        /// Gets and filters materials.
        /// </summary>
        /// <param name="partialName">A string to filter material names with.</param>
        /// <param name="partialManufacturerName">A string to filter manufacturers with.</param>
        /// <param name="type">The type to filter with.</param>
        /// <returns>Returns the matching materials.</returns>
        IEnumerable<Material> GetFilteredMaterials(string partialName = null, string partialManufacturerName = null, MaterialType? type = null);
    }
}
