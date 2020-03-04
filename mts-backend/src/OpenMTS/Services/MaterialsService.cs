using Microsoft.Extensions.Logging;
using OpenMTS.Models;
using OpenMTS.Repositories;
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
        /// A logger for local logging needs.
        /// </summary>
        /// <value>
        private ILogger Logger { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="MaterialsService"/> class.
        /// </summary>
        /// <param name="loggerFactory">A factory to create loggers from.</param>
        /// <param name="materialsRepository">A materials repository.</param>
        public MaterialsService(ILoggerFactory loggerFactory, IMaterialsRepository materialsRepository)
        {
            Logger = loggerFactory.CreateLogger<MaterialsService>();
            MaterialsRepository = materialsRepository;
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
        public IEnumerable<Material> GetMaterials(string partialName, string partialManufacturerName, MaterialType? materialType)
        {
            return MaterialsRepository.GetFilteredMaterials(partialName, partialManufacturerName, materialType);
        }
    }
}
