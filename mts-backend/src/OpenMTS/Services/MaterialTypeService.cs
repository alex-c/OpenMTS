using Microsoft.Extensions.Logging;
using OpenMTS.Models;
using OpenMTS.Repositories;
using OpenMTS.Services.Exceptions;
using System.Collections.Generic;

namespace OpenMTS.Services
{
    /// <summary>
    /// A service exposing functionality relating to material types.
    /// </summary>
    public class MaterialTypeService
    {
        /// <summary>
        /// The underlying repository of material types.
        /// </summary>
        private IMaterialTypeRepository MaterialTypeRepository { get; }

        /// <summary>
        /// A logger for local logging needs.
        /// </summary>
        private ILogger Logger { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="MaterialTypeService"/> class.
        /// </summary>
        /// <param name="loggerFactory">A factory to create loggers from.</param>
        /// <param name="materialTypeRepository">A material type repository.</param>
        public MaterialTypeService(ILoggerFactory loggerFactory, IMaterialTypeRepository materialTypeRepository)
        {
            Logger = loggerFactory.CreateLogger<MaterialTypeService>();
            MaterialTypeRepository = materialTypeRepository;
        }

        /// <summary>
        /// Gets all available material types.
        /// </summary>
        /// <returns>Returns all material types.</returns>
        public IEnumerable<MaterialType> GetMaterialTypes()
        {
            return MaterialTypeRepository.GetAllMaterialTypes();
        }

        /// <summary>
        /// Gets and filters material types using a partial name.
        /// </summary>
        /// <param name="partialName">String to filter with..</param>
        /// <returns>Returns filtered material types.</returns>
        public IEnumerable<MaterialType> GetMaterialTypes(string partialName)
        {
            return MaterialTypeRepository.SearchMaterialTypesByName(partialName);
        }

        /// <summary>
        /// Gets a material type.
        /// </summary>
        /// <param name="id">The ID of the material type to get.</param>
        /// <returns>Returns the material type or null.</returns>
        /// <exception cref="MaterialTypeNotFoundException">Thrown if no matching material type could not be found.</exception>
        public MaterialType GetMaterialType(string id)
        {
            return GetMaterialTypeOrThrowNotFoundException(id);
        }

        /// <summary>
        /// Creates a new material type.
        /// </summary>
        /// <param name="id">The ID of the new material type.</param>
        /// <param name="name">The name of the material type to create.</param>
        /// <returns>Returns the newly created material type.</returns>
        public MaterialType CreateMaterialType(string id, string name)
        {
            return MaterialTypeRepository.CreateMaterialType(id, name);
        }

        /// <summary>
        /// Updates a material type.
        /// </summary>
        /// <param name="materialType">The material type to update.</param>
        /// <returns>Returns the updated material type.</returns>
        /// <exception cref="MaterialTypeNotFoundException">Thrown if no matching material type could not be found.</exception>
        public MaterialType UpdateMaterialType(string id, string name)
        {
            MaterialType materialType = GetMaterialTypeOrThrowNotFoundException(id);
            materialType.Name = name;
            MaterialTypeRepository.UpdateMaterialType(materialType);
            return materialType;
        }

        #region Private helpers

        /// <summary>
        /// Attempts to get a material type from the underlying repository and throws a <see cref="MaterialTypeNotFoundException"/> if no matching type could be found.
        /// </summary>
        /// <param name="id">ID of the type to get.</param>
        /// <exception cref="MaterialTypeNotFoundException">Thrown if no matching type could be found.</exception>
        /// <returns>Returns the type, if found.</returns>
        private MaterialType GetMaterialTypeOrThrowNotFoundException(string id)
        {
            MaterialType type = MaterialTypeRepository.GetMaterialType(id);
            if (type == null)
            {
                throw new MaterialTypeNotFoundException($"Could not find any material type with Id `${id}`.");
            }
            return type;
        }

        #endregion
    }
}
