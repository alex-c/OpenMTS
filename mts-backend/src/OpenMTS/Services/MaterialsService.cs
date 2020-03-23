using Microsoft.Extensions.Logging;
using OpenMTS.Models;
using OpenMTS.Repositories;
using OpenMTS.Services.Exceptions;
using System.Collections.Generic;

namespace OpenMTS.Services
{
    /// <summary>
    /// Gants access to materials and materials administration functionality.
    /// </summary>
    public class MaterialsService
    {
        /// <summary>
        /// The underlying repository granting access to materials.
        /// </summary>
        private IMaterialsRepository MaterialsRepository { get; }

        /// <summary>
        /// Persistence of custom material prop values.
        /// </summary>
        private ICustomMaterialPropValueRepository CustomMaterialPropValueRepository { get; }

        /// <summary>
        /// A logger for local logging needs.
        /// </summary>
        /// <value>
        private ILogger Logger { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="MaterialsService"/> class.
        /// </summary>
        /// <param name="loggerFactory">A factory to create loggers from.</param>
        /// <param name="materialsRepository">A materials repository.</param>
        public MaterialsService(ILoggerFactory loggerFactory,
            IMaterialsRepository materialsRepository,
            ICustomMaterialPropValueRepository customMaterialPropValueRepository)
        {
            Logger = loggerFactory.CreateLogger<MaterialsService>();
            MaterialsRepository = materialsRepository;
            CustomMaterialPropValueRepository = customMaterialPropValueRepository;
        }

        /// <summary>
        /// Gets all available materials.
        /// </summary>
        /// <returns>Reutrns all materials.</returns>
        public IEnumerable<Material> GetAllMaterials()
        {
            return MaterialsRepository.GetAllMaterials();
        }

        /// <summary>
        /// Gets and filters materials.
        /// </summary>
        /// <param name="partialName">A string to filter material names with.</param>
        /// <param name="partialManufacturerName">A string to filter manufacturers with.</param>
        /// <param name="materialType">Type of the material to filter with.</param>
        /// <returns>Returns a filtered list of materials</returns>
        public IEnumerable<Material> GetMaterials(string partialName, string partialManufacturerName, MaterialType materialType)
        {
            return MaterialsRepository.GetFilteredMaterials(partialName, partialManufacturerName, materialType);
        }

        /// <summary>
        /// Gets a material by its ID.
        /// </summary>
        /// <param name="id">ID of the material to get.</param>
        /// <returns>Returns the material or null.</returns>
        /// <exception cref="MaterialNotFoundException">Thrown if no matching material could be found.</exception>
        public Material GetMaterial(int id)
        {
            return GetMaterialOrThrowNotFoundException(id);
        }

        /// <summary>
        /// Creates a new material.
        /// </summary>
        /// <param name="name">The new material's name.</param>
        /// <param name="manufacturerName">Name of the manufacturer.</param>
        /// <param name="manufacturerSpecificId">The manufacturer's ID for this material.</param>
        /// <param name="materialType">Type of the material.</param>
        /// <returns>Retursn the newly created material.</returns>
        public Material CreateMaterial(string name, string manufacturerName, string manufacturerSpecificId, MaterialType materialType)
        {
            return MaterialsRepository.CreateMaterial(name, manufacturerName, manufacturerSpecificId, materialType);
        }

        /// <summary>
        /// Updates an existing material's master data.
        /// </summary>
        /// <param name="id">ID of the material to update.</param>
        /// <param name="name">The material's name.</param>
        /// <param name="manufacturerName">Name of the manufacturer.</param>
        /// <param name="manufacturerSpecificId">The manufacturer's ID for this material.</param>
        /// <param name="materialType">Type of the material.</param>
        /// <returns>Retursn the updated material.</returns>
        /// <exception cref="MaterialNotFoundException">Thrown if no matching material could be found.</exception>
        public Material UpdateMaterial(int id, string name, string manufacturerName, string manufacturerSpecificId, MaterialType materialType)
        {
            Material material = GetMaterialOrThrowNotFoundException(id);
            material.Name = name;
            material.Manufacturer = manufacturerName;
            material.ManufacturerSpecificId = manufacturerSpecificId;
            material.Type = materialType;
            MaterialsRepository.UpdateMaterial(material);
            return material;
        }

        #region Custom props

        /// <summary>
        /// Sets or removes a custom material prop value of the text type.
        /// </summary>
        /// <param name="id">The material ID.</param>
        /// <param name="prop">The custom prop ID.</param>
        /// <param name="text">The text to set, or null.</param>
        public void UpdateCustomTextMaterialProp(int id, CustomMaterialProp prop, string text)
        {
            // Make sure the material exists
            GetMaterialOrThrowNotFoundException(id);

            // Proceed
            if (text == null)
            {
                CustomMaterialPropValueRepository.RemoveCustomTextMaterialProp(id, prop.Id);
            }
            else
            {
                CustomMaterialPropValueRepository.SetCustomTextMaterialProp(id, prop.Id, text);
            }
        }

        #endregion

        #region Private Helpers

        /// <summary>
        /// Attempts to get a material from the underlying repository and throws a <see cref="MaterialNotFoundException"/> if no matching material could be found.
        /// </summary>
        /// <param name="id">ID of the material to get.</param>
        /// <exception cref="MaterialNotFoundException">Thrown if no matching material could be found.</exception>
        /// <returns>Returns the material, if found.</returns>
        private Material GetMaterialOrThrowNotFoundException(int id)
        {
            Material material = MaterialsRepository.GetMaterial(id);

            // Check for material existence
            if (material == null)
            {
                throw new MaterialNotFoundException(id);
            }

            return material;
        }

        #endregion
    }
}
