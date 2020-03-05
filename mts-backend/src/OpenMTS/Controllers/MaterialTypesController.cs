using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OpenMTS.Controllers.Contracts.Responses;
using OpenMTS.Models;
using OpenMTS.Services;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OpenMTS.Controllers
{
    /// <summary>
    /// Endpoint for data access and administration of types of materials.
    /// </summary>
    /// <seealso cref="OpenMTS.Controllers.ControllerBase" />
    [Route("api/material-types")]
    public class MaterialTypesController : ControllerBase
    {
        /// <summary>
        /// The service granting access to material type-related functionality.
        /// </summary>
        /// <value>
        private MaterialTypeService MaterialTypeService { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="MaterialTypesController"/> class.
        /// </summary>
        /// <param name="loggerFactory">A factory to create loggers from.</param>
        public MaterialTypesController(ILoggerFactory loggerFactory, MaterialTypeService materialTypeService)
        {
            Logger = loggerFactory.CreateLogger<MaterialTypesController>();
            MaterialTypeService = materialTypeService;
        }

        /// <summary>
        /// Gets paginated material types.
        /// </summary>
        /// <param name="page">The page to display.</param>
        /// <param name="elementsPerPage">The number of elements to display per page.</param>
        /// <param name="search">An optional search string to filter material type names with.</param>
        /// <returns>Returns the filtered and paginated material types.</returns>
        [HttpGet]
        public IActionResult GetMaterialTypes([FromQuery] int page = 1, [FromQuery] int elementsPerPage = 10, [FromQuery] string search = null)
        {
            try
            {
                IEnumerable<MaterialType> materialTypes = null;
                if (search == null)
                {
                    materialTypes = MaterialTypeService.GetMaterialTypes();
                }
                else
                {
                    materialTypes = MaterialTypeService.GetMaterialTypes(search);
                }
                
                IEnumerable<MaterialType> paginatedMaterialTypes = materialTypes.Skip((page - 1) * elementsPerPage).Take(elementsPerPage);
                return Ok(new PaginatedResponse(paginatedMaterialTypes, materialTypes.Count()));
            }
            catch (Exception exception)
            {
                return HandleUnexpectedException(exception);
            }

        }
    }
}
