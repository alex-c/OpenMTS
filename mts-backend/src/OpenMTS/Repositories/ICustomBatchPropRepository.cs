using OpenMTS.Models;
using System;
using System.Collections.Generic;

namespace OpenMTS.Repositories
{
    /// <summary>
    /// A repository of custom batch properties.
    /// </summary>
    public interface ICustomBatchPropRepository
    {
        /// <summary>
        /// Gets all custom batch properties.
        /// </summary>
        /// <returns>Returns a list of props.</returns>
        IEnumerable<CustomBatchProp> GetAllCustomBatchProps();

        /// <summary>
        /// Gets a custom batch property.
        /// </summary>
        /// <param name="id">The property's ID.</param>
        /// <returns>Returns the property or null.</returns>
        CustomBatchProp GetCustomBatchProp(Guid id);

        /// <summary>
        /// Creates a custom batch property.
        /// </summary>
        /// <param name="name">The property's name.</param>
        /// <returns>Returns the newly created prop.</returns>
        CustomBatchProp CreateCustomBatchProp(string name);

        /// <summary>
        /// Updates a custom batch property.
        /// </summary>
        /// <param name="prop">Prop to update.</param>
        void UpdateCustomBatchProp(CustomBatchProp prop);

        /// <summary>
        /// Deletes a custom batch property.
        /// </summary>
        /// <param name="id">The ID of the prop to delete.</param>
        void DeleteCustomBatchProp(Guid id);
    }
}
