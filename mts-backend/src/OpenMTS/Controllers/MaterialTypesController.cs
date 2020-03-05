using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OpenMTS.Controllers.Contracts.Requests;
using OpenMTS.Controllers.Contracts.Responses;
using OpenMTS.Models;
using OpenMTS.Services;
using OpenMTS.Services.Exceptions;
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
        /// <param name="getAll">Disables pagination - all elements will be returned.</param>
        /// <param name="page">The page to display.</param>
        /// <param name="elementsPerPage">The number of elements to display per page.</param>
        /// <param name="search">An optional search string to filter material type names with.</param>
        /// <returns>Returns the filtered and paginated material types.</returns>
        [HttpGet]
        public IActionResult GetMaterialTypes([FromQuery] bool getAll = false, [FromQuery] int page = 0, [FromQuery] int elementsPerPage = 10, [FromQuery] string search = null)
        {
            if (!getAll && (page < 1 || elementsPerPage < 1))
            {
                return HandleBadRequest("Bad pagination parameters.");
            }

            try
            {
                // Get (potentially filtered) types and order alphabetically by ID
                IEnumerable<MaterialType> materialTypes = null;
                if (search == null)
                {
                    materialTypes = MaterialTypeService.GetMaterialTypes();
                }
                else
                {
                    materialTypes = MaterialTypeService.GetMaterialTypes(search);
                }
                materialTypes = materialTypes.OrderBy(m => m.Id);

                // Paginate if needed
                IEnumerable<MaterialType> paginatedMaterialTypes = materialTypes;
                if (!getAll)
                {
                    paginatedMaterialTypes = materialTypes.Skip((page - 1) * elementsPerPage).Take(elementsPerPage);
                }
                
                // Done!
                return Ok(new PaginatedResponse(paginatedMaterialTypes, materialTypes.Count()));
            }
            catch (Exception exception)
            {
                return HandleUnexpectedException(exception);
            }

        }

        /// <summary>
        /// Creates a new type of the material.
        /// </summary>
        /// <param name="createMaterialTypeRequest">The request to create a new material type.</param>
        /// <returns>Returns a `201 Created` response on success.</returns>
        [HttpPost]
        public IActionResult CreateMaterialType([FromBody] CreateMaterialTypeRequest createMaterialTypeRequest)
        {
            if (createMaterialTypeRequest == null ||
                string.IsNullOrWhiteSpace(createMaterialTypeRequest.Id) ||
                string.IsNullOrWhiteSpace(createMaterialTypeRequest.Name))
            {
                return HandleBadRequest("A valid material type creation request with an ID and name must be supplied.");
            }

            try
            {
                MaterialType type = MaterialTypeService.CreateMaterialType(createMaterialTypeRequest.Id, createMaterialTypeRequest.Name);
                return Created(GetNewResourceUri(createMaterialTypeRequest.Id), type);
            }
            catch (MaterialTypeAlreadyExistsException exception)
            {
                return HandleResourceAlreadyExistsException(exception);
            }
            catch (Exception exception)
            {
                return HandleUnexpectedException(exception);
            }
        }

        /// <summary>
        /// Updates a material type.
        /// </summary>
        /// <param name="id">The ID of the type to update.</param>
        /// <param name="updateMaterialTypeRequest">The data to update.</param>
        /// <returns>Returns the updated type.</returns>
        [HttpPatch("{id}")]
        public IActionResult UpdateMaterialType(string id, [FromBody] UpdateMaterialTypeRequest updateMaterialTypeRequest)
        {
            if (updateMaterialTypeRequest == null ||
                string.IsNullOrWhiteSpace(updateMaterialTypeRequest.Name))
            {
                return HandleBadRequest("A valid material type update request with a name must be supplied.");
            }

            try
            {
                return Ok(MaterialTypeService.UpdateMaterialType(id, updateMaterialTypeRequest.Name));
            }
            catch (MaterialTypeNotFoundException exception)
            {
                return HandleResourceNotFoundException(exception);
            }
            catch (Exception exception)
            {
                return HandleUnexpectedException(exception);
            }
        }
    }
}
