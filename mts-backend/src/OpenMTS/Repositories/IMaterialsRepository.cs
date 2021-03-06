﻿using OpenMTS.Models;
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
        IEnumerable<Material> GetFilteredMaterials(string partialName = null, string partialManufacturerName = null, Plastic type = null);

        /// <summary>
        /// Gets a material by it's ID.
        /// </summary>
        /// <param name="id">The ID of the material to get.</param>
        /// <returns>Returns the material or null.</returns>
        Material GetMaterial(int id);

        /// <summary>
        /// Creates a new material.
        /// </summary>
        /// <param name="name">The new material's name.</param>
        /// <param name="manufacturerName">Name of the manufacturer.</param>
        /// <param name="manufacturerSpecificId">The manufacturer's ID for this material.</param>
        /// <param name="materialType">Type of the material.</param>
        /// <returns>Retursn the newly created material.</returns>
        Material CreateMaterial(string name, string manufacturerName, string manufacturerSpecificId, Plastic type);

        /// <summary>
        /// Updates an existing material.
        /// </summary>
        /// <param name="material">The material to update.</param>
        void UpdateMaterial(Material material);
    }
}
