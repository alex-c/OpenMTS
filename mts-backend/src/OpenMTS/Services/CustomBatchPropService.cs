using Microsoft.Extensions.Logging;
using OpenMTS.Models;
using OpenMTS.Repositories;
using OpenMTS.Services.Exceptions;
using System;
using System.Collections.Generic;

namespace OpenMTS.Services
{
    /// <summary>
    /// Service exposing query and administration features for custom material batch properties.
    /// </summary>
    public class CustomBatchPropService
    {
        /// <summary>
        /// The underlying repository for custom props.
        /// </summary>
        private ICustomBatchPropRepository PropRepository { get; }

        /// <summary>
        /// A logger for local logging needs
        /// </summary>
        private ILogger Logger { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomBatchPropService"/> class.
        /// </summary>
        /// <param name="loggerFactory">A factory to create loggers from.</param>
        /// <param name="customBatchPropRepository">The custom batch property repository.</param>
        public CustomBatchPropService(ILoggerFactory loggerFactory, ICustomBatchPropRepository customBatchPropRepository)
        {
            Logger = loggerFactory.CreateLogger<CustomBatchPropService>();
            PropRepository = customBatchPropRepository;
        }

        /// <summary>
        /// Gets all custom batch properties.
        /// </summary>
        /// <returns>Returns a list of props.</returns>
        public IEnumerable<CustomBatchProp> GetAllCustomBatchProps()
        {
            return PropRepository.GetAllCustomBatchProps();
        }

        /// <summary>
        /// Gets a custom batch property.
        /// </summary>
        /// <param name="id">The property's ID.</param>
        /// <returns>Returns the property or null.</returns>
        /// <exception cref="CustomPropNotFoundException">Thrown if no matching prop could be found.</exception>
        public CustomBatchProp GetCustomBatchProp(Guid id)
        {
            return GetCustomBatchPropOrThrowNotFoundException(id);
        }

        /// <summary>
        /// Creates a custom batch property.
        /// </summary>
        /// <param name="name">The property's name.</param>
        /// <returns>Returns the newly created prop.</returns>
        public CustomBatchProp CreateCustomBatchProp(string name)
        {
            return PropRepository.CreateCustomBatchProp(name);
        }

        /// <summary>
        /// Updates a custom batch property.
        /// </summary>
        /// <param name="name">The new name to set.</param>
        /// <returns>Returns the updated prop.</returns>
        /// <exception cref="CustomPropNotFoundException">Thrown if no matching prop could be found.</exception>
        public CustomBatchProp UpdateCustomBatchProp(Guid id, string name)
        {
            CustomBatchProp prop = GetCustomBatchPropOrThrowNotFoundException(id);
            prop.Name = name;
            PropRepository.UpdateCustomBatchProp(prop);
            return prop;
        }

        /// <summary>
        /// Deletes a custom batch property.
        /// </summary>
        /// <param name="id">The ID of the prop to delete.</param>
        public void DeleteCustomBatchProp(Guid id)
        {
            PropRepository.DeleteCustomBatchProp(id);
        }

        #region Private Helpers

        /// <summary>
        /// Attempts to get a batch prop from the underlying repository and throws a <see cref="CustomPropNotFoundException"/> if no matching prop could be found.
        /// </summary>
        /// <param name="id">ID of the prop to get.</param>
        /// <exception cref="CustomPropNotFoundException">Thrown if no matching prop could be found.</exception>
        /// <returns>Returns the prop, if found.</returns>
        private CustomBatchProp GetCustomBatchPropOrThrowNotFoundException(Guid id)
        {
            CustomBatchProp prop = PropRepository.GetCustomBatchProp(id);

            // Check for prop existence
            if (prop == null)
            {
                throw new CustomPropNotFoundException(id);
            }

            return prop;
        }

        #endregion
    }
}
