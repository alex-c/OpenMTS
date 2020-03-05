using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OpenMTS.Services;
using System.Collections.Generic;
using System.Linq;

namespace OpenMTS.Controllers
{
    /// <summary>
    /// Router for data and functionality related to material manufacturers.
    /// </summary>
    /// <seealso cref="OpenMTS.Controllers.ControllerBase" />
    [Route("api/manufacturers"), Authorize]
    public class ManufacturersController : ControllerBase
    {
        /// <summary>
        /// The materials service from which manufacturer data is gathered.
        /// </summary>
        private MaterialsService MaterialsService { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ManufacturersController"/> class.
        /// </summary>
        /// <param name="loggerFactory">A factory to create loggers from.</param>
        /// <param name="materialsService">The materials service to gather manufacturer data from.</param>
        public ManufacturersController(ILoggerFactory loggerFactory, MaterialsService materialsService)
        {
            Logger = loggerFactory.CreateLogger<ManufacturersController>();
            MaterialsService = materialsService;
        }

        /// <summary>
        /// Gets all known manufacturers.
        /// </summary>
        /// <returns>Returns a list of unique manufactuer names.</returns>
        [HttpGet]
        public IActionResult GetManufacturers()
        {
            IEnumerable<string> manufacturers = MaterialsService.GetAllMaterials().Select(m => m.Manufacturer).OrderBy(m => m);
            ISet<string> uniqueManufactuers = new HashSet<string>(manufacturers);
            return Ok(uniqueManufactuers);
        }
    }
}
