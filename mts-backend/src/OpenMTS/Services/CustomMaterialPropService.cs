using Microsoft.Extensions.Logging;
using OpenMTS.Models;
using OpenMTS.Repositories;
using OpenMTS.Services.Exceptions;
using System;
using System.Collections.Generic;

namespace OpenMTS.Services
{
    /// <summary>
    /// A service for querying and managing custom material properties.
    /// </summary>
    public class CustomMaterialPropService
    {
        /// <summary>
        /// The underlying repository for custom props.
        /// </summary>
        private ICustomMaterialPropRepository PropRepository { get; }

        /// <summary>
        /// A logger for local logging needs.
        /// </summary>
        private ILogger Logger { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomMaterialPropService"/> class.
        /// </summary>
        /// <param name="loggerFactory">A factory to create loggers from.</param>
        /// <param name="propRepository">The property repository.</param>
        public CustomMaterialPropService(ILoggerFactory loggerFactory, ICustomMaterialPropRepository propRepository)
        {
            Logger = loggerFactory.CreateLogger<CustomMaterialPropService>();
            PropRepository = propRepository;
        }

        /// <summary>
        /// Gets all custom material properties.
        /// </summary>
        /// <returns>Returns a list of props.</returns>
        public IEnumerable<CustomMaterialProp> GetAllCustomMaterialProps()
        {
            return PropRepository.GetAllCustomMaterialProps();
        }

        /// <summary>
        /// Gets a custom material property.
        /// </summary>
        /// <param name="id">The property's ID.</param>
        /// <returns>Returns the property or null.</returns>
        /// <exception cref="CustomPropNotFoundException">Thrown if no matching prop could be found.</exception>
        public CustomMaterialProp GetCustomMaterialProp(Guid id)
        {
            return GetApiKeyOrThrowNotFoundException(id);
        }

        /// <summary>
        /// Creates a custom material property.
        /// </summary>
        /// <param name="name">The property's name.</param>
        /// <param name="type">The property's type.</param>
        /// <returns>Returns the newly created prop.</returns>
        public CustomMaterialProp CreateCustomMaterialProp(string name, PropType type)
        {
            return PropRepository.CreateCustomMaterialProp(name, type);
        }

        /// <summary>
        /// Updates a custom material property.
        /// </summary>
        /// <param name="name">The new name to set.</param>
        /// <returns>Returns the updated prop.</returns>
        /// <exception cref="CustomPropNotFoundException">Thrown if no matching prop could be found.</exception>
        public CustomMaterialProp UpdateCustomMaterialProp(Guid id, string name)
        {
            CustomMaterialProp prop = GetApiKeyOrThrowNotFoundException(id);
            prop.Name = name;
            PropRepository.UpdateCustomMaterialProp(prop);
            return prop;
        }

        /// <summary>
        /// Deletes a custom material property.
        /// </summary>
        /// <param name="id">The ID of the prop to delete.</param>
        public void DeleteCustomMaterialProp(Guid id)
        {
            PropRepository.DeleteCustomMaterialProp(id);
        }

        #region Private Helpers

        /// <summary>
        /// Attempts to get a material prop from the underlying repository and throws a <see cref="CustomPropNotFoundException"/> if no matching prop could be found.
        /// </summary>
        /// <param name="id">ID of the prop to get.</param>
        /// <exception cref="CustomPropNotFoundException">Thrown if no matching prop could be found.</exception>
        /// <returns>Returns the prop, if found.</returns>
        private CustomMaterialProp GetApiKeyOrThrowNotFoundException(Guid id)
        {
            CustomMaterialProp prop = PropRepository.GetCustomMaterialProp(id);

            // Check for key existence
            if (prop == null)
            {
                throw new CustomPropNotFoundException(id);
            }

            return prop;
        }

        #endregion
    }
}
