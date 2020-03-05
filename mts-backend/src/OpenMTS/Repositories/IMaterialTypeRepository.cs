using OpenMTS.Models;
using System.Collections.Generic;

namespace OpenMTS.Repositories
{
    /// <summary>
    /// A generic interface for a repository of material types.
    /// </summary>
    public interface IMaterialTypeRepository
    {
        /// <summary>
        /// Gets all available material types.
        /// </summary>
        /// <returns>Returns all material types.</returns>
        IEnumerable<MaterialType> GetAllMaterialTypes();

        /// <summary>
        /// Gets and filters material types using a partial name.
        /// </summary>
        /// <param name="partialName">String to filter with..</param>
        /// <returns>Returns filtered material types.</returns>
        IEnumerable<MaterialType> SearchMaterialTypesByName(string partialName);

        /// <summary>
        /// Gets a material type.
        /// </summary>
        /// <param name="id">The ID of the material type to get.</param>
        /// <returns>Returns the material type or null.</returns>
        MaterialType GetMaterialType(string id);

        /// <summary>
        /// Creates a new material type.
        /// </summary>
        /// <param name="id">The ID of the new material type.</param>
        /// <param name="name">The name of the material type to create.</param>
        /// <returns>Returns the newly created material type.</returns>
        MaterialType CreateMaterialType(string id, string name);

        /// <summary>
        /// Updates a material type.
        /// </summary>
        /// <param name="materialType">The material type to update.</param>
        void UpdateMaterialType(MaterialType materialType);

        /// <summary>
        /// Deletes a material type.
        /// </summary>
        /// <param name="id">The ID of the material type to delete.</param>
        void DeleteMaterialType(string id);
    }
}
