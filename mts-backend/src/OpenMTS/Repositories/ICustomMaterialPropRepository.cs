using OpenMTS.Models;
using System;
using System.Collections.Generic;

namespace OpenMTS.Repositories
{
    /// <summary>
    /// A repository for custom material properties.
    /// </summary>
    public interface ICustomMaterialPropRepository
    {
        /// <summary>
        /// Gets all custom material properties.
        /// </summary>
        /// <returns>Returns a list of props.</returns>
        IEnumerable<CustomMaterialProp> GetAllCustomMaterialProps();

        /// <summary>
        /// Gets a custom material property.
        /// </summary>
        /// <param name="id">The property's ID.</param>
        /// <returns>Returns the property or null.</returns>
        CustomMaterialProp GetCustomMaterialProp(Guid id);

        /// <summary>
        /// Creates a custom material property.
        /// </summary>
        /// <param name="name">The property's name.</param>
        /// <param name="type">The property's type.</param>
        /// <returns>Returns the newly created prop.</returns>
        CustomMaterialProp CreateCustomMaterialProp(string name, PropType type);

        /// <summary>
        /// Updates a custom material property.
        /// </summary>
        /// <param name="prop">Prop to update.</param>
        void UpdateCustomMaterialProp(CustomMaterialProp prop);

        /// <summary>
        /// Deletes a custom material property.
        /// </summary>
        /// <param name="id">The ID of the prop to delete.</param>
        void DeleteCustomMaterialProp(Guid id);
    }
}
