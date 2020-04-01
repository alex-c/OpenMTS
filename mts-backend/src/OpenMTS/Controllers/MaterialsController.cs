using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Logging;
using OpenMTS.Authorization;
using OpenMTS.Controllers.Contracts.Requests;
using OpenMTS.Controllers.Contracts.Responses;
using OpenMTS.Models;
using OpenMTS.Services;
using OpenMTS.Services.Exceptions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace OpenMTS.Controllers
{
    /// <summary>
    /// A route granting access to materials data and administration.
    /// </summary>
    /// <seealso cref="OpenMTS.Controllers.ControllerBase" />
    [Route("api/materials"), Authorize]
    public class MaterialsController : ControllerBase
    {
        /// <summary>
        /// A service for plastics functionality.
        /// </summary>
        private PlasticsService PlasticsService { get; }

        /// <summary>
        /// The underlying service for materials functionality.
        /// </summary>
        private MaterialsService MaterialsService { get; }

        /// <summary>
        /// Allows to get configured custom material properties.
        /// </summar
        private CustomMaterialPropService CustomMaterialPropService { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="MaterialsController"/> class.
        /// </summary>
        /// <param name="loggerFactory">A factory to create loggers from.</param>
        /// <param name="plasticsService">Grants access to known plastics.</param>
        /// <param name="materialsService">The materials service.</param>
        /// <param name="customMaterialPropService">Grants access to custom material props.</param>
        public MaterialsController(ILoggerFactory loggerFactory,
            PlasticsService plasticsService,
            MaterialsService materialsService,
            CustomMaterialPropService customMaterialPropService)
        {
            Logger = loggerFactory.CreateLogger<MaterialsController>();
            PlasticsService = plasticsService;
            MaterialsService = materialsService;
            CustomMaterialPropService = customMaterialPropService;
        }

        /// <summary>
        /// Gets, filters and paginates materials.
        /// </summary>
        /// <param name="getAll">Disables pagination - all elements will be returned.</param>
        /// <param name="page">The page to display.</param>
        /// <param name="elementsPerPage">The number of elements to display per page.</param>
        /// <param name="search">A string to search in materials names.</param>
        /// <param name="manufacturer">The manufacturer to filter with.</param>
        /// <param name="type">Type of the material to filter with.</param>
        /// <returns>Returns a filtered, paginated list of materials.</returns>
        [HttpGet]
        public IActionResult GetMaterials(
            [FromQuery] bool getAll = false,
            [FromQuery] int page = 1,
            [FromQuery] int elementsPerPage = 10,
            [FromQuery] string search = null,
            [FromQuery] string manufacturer = null,
            [FromQuery] string type = null)
        {
            if (!getAll && (page < 1 || elementsPerPage < 1))
            {
                return HandleBadRequest("Bad pagination parameters.");
            }

            // Check for material type validity
            Plastic plastic = null;
            if (type != null)
            {
                try
                {
                    plastic = PlasticsService.GetPlastic(type);
                }
                catch (PlasticNotFoundException exception)
                {
                    return HandleBadRequest(exception.Message);
                }
            }

            // Get, filter and paginate materials
            try
            {
                IEnumerable<Material> materials = MaterialsService.GetMaterials(search, manufacturer, plastic);
                IEnumerable<Material> paginatedMaterials = materials;
                if (!getAll)
                {
                    paginatedMaterials = materials.Skip((page - 1) * elementsPerPage).Take(elementsPerPage);
                }
                return Ok(new PaginatedResponse(paginatedMaterials, materials.Count()));
            }
            catch (Exception exception)
            {
                return HandleUnexpectedException(exception);
            }
        }

        /// <summary>
        /// Gets an existing material.
        /// </summary>
        /// <param name="id">The ID of the material to get.</param>
        /// <returns>Returns the material</returns>
        [HttpGet("{id}")]
        public IActionResult GetMaterial(int id)
        {
            try
            {
                Material material = MaterialsService.GetMaterial(id);
                return Ok(material);
            }
            catch (MaterialNotFoundException exception)
            {
                return HandleResourceNotFoundException(exception);
            }
            catch (Exception exception)
            {
                return HandleUnexpectedException(exception);
            }
        }

        /// <summary>
        /// Creates a new material.
        /// </summary>
        /// <param name="materialCreationRequest">The material creation request.</param>
        /// <returns>Returns a `201 Created` response on success.</returns>
        [HttpPost, Authorize(Policy = AuthPolicyNames.MAY_CREATE_MATERIAL)]
        public IActionResult CreateMaterial([FromBody] MaterialCreationRequest materialCreationRequest)
        {
            if (materialCreationRequest == null ||
                string.IsNullOrWhiteSpace(materialCreationRequest.Name) ||
                string.IsNullOrWhiteSpace(materialCreationRequest.Manufacturer) ||
                string.IsNullOrWhiteSpace(materialCreationRequest.ManufacturerId) ||
                string.IsNullOrWhiteSpace(materialCreationRequest.Type))
            {
                return HandleBadRequest("Incomplete or invalid material data submitted for creation.");
            }

            // Check for material type validity
            Plastic plastic = null;
            try
            {
                plastic = PlasticsService.GetPlastic(materialCreationRequest.Type);
            }
            catch (PlasticNotFoundException exception)
            {
                return HandleBadRequest(exception.Message);
            }

            // Proceed with creation
            try
            {
                Material material = MaterialsService.CreateMaterial(materialCreationRequest.Name, materialCreationRequest.Manufacturer, materialCreationRequest.ManufacturerId, plastic);
                return Created(GetNewResourceUri(material.Id), material);
            }
            catch (Exception exception)
            {
                return HandleUnexpectedException(exception);
            }
        }

        /// <summary>
        /// Updates an existing material's master data.
        /// </summary>
        /// <param name="id">The ID of the material to update.</param>
        /// <param name="materialUpdateRequest">The material update request data.</param>
        /// <returns>Returns the updated material on success.</returns>
        [HttpPatch("{id}"), Authorize(Policy = AuthPolicyNames.MAY_UPDATE_MATERIAL)]
        public IActionResult UpdateMaterialMasterData(int id, [FromBody] MaterialUpdateRequest materialUpdateRequest)
        {
            if (materialUpdateRequest == null ||
                string.IsNullOrWhiteSpace(materialUpdateRequest.Name) ||
                string.IsNullOrWhiteSpace(materialUpdateRequest.Manufacturer) ||
                string.IsNullOrWhiteSpace(materialUpdateRequest.ManufacturerId) ||
                string.IsNullOrWhiteSpace(materialUpdateRequest.Type))
            {
                return HandleBadRequest("Incomplete or invalid material data submitted for update.");
            }

            // Check for material type validity
            Plastic plastic = null;
            try
            {
                plastic = PlasticsService.GetPlastic(materialUpdateRequest.Type);
            }
            catch (PlasticNotFoundException exception)
            {
                return HandleBadRequest(exception.Message);
            }

            // Proceed with updating
            try
            {
                Material material = MaterialsService.UpdateMaterial(id, materialUpdateRequest.Name, materialUpdateRequest.Manufacturer, materialUpdateRequest.ManufacturerId, plastic);
                return Ok(material);
            }
            catch (MaterialNotFoundException exception)
            {
                return HandleResourceNotFoundException(exception);
            }
            catch (Exception exception)
            {
                return HandleUnexpectedException(exception);
            }

        }

        #region Custom properties

        /// <summary>
        /// Sets a custom material prop value of the text type.
        /// </summary>
        /// <param name="materialId">The ID of the material.</param>
        /// <param name="propId">The ID of the prop to set.</param>
        /// <param name="setCustomTextMaterialPropRequest">Contains the prop value to set.</param>
        /// <returns>Returns a `204 No Content` response on success.</returns>
        [HttpPut("{materialId}/text-props/{propId}"), Authorize(Policy = AuthPolicyNames.MAY_SET_CUSTOM_MATERIAL_PROP_VALUE)]
        public IActionResult SetCustomTextMaterialProp(int materialId, Guid propId, [FromBody] SetCustomTextMaterialPropRequest setCustomTextMaterialPropRequest)
        {
            if (setCustomTextMaterialPropRequest == null ||
                string.IsNullOrWhiteSpace(setCustomTextMaterialPropRequest.Text))
            {
                return HandleBadRequest("A text to set for the custom material prop has to be provided.");
            }

            try
            {
                // Get custom prop and validate type
                CustomMaterialProp prop = CustomMaterialPropService.GetCustomMaterialProp(propId);
                if (prop.Type != PropType.Text)
                {
                    return HandleBadRequest("The submitted prop is not of the type `text`.");
                }

                // Update material - set prop
                MaterialsService.UpdateCustomTextMaterialProp(materialId, prop, setCustomTextMaterialPropRequest.Text);

                // Done!
                return NoContent();
            }
            catch (CustomPropNotFoundException exception)
            {
                return HandleResourceNotFoundException(exception);
            }
            catch (MaterialNotFoundException exception)
            {
                return HandleResourceNotFoundException(exception);
            }
            catch (Exception exception)
            {
                return HandleUnexpectedException(exception);
            }
        }

        /// <summary>
        /// Deletes a custom material prop value of the text type.
        /// </summary>
        /// <param name="materialId">The ID of the material.</param>
        /// <param name="propId">The ID of the prop to unset.</param>
        /// <returns>Returns a `204 No Content` response on success.</returns>
        [HttpDelete("{materialId}/text-props/{propId}"), Authorize(Policy = AuthPolicyNames.MAY_DELETE_CUSTOM_MATERIAL_PROP_VALUE)]
        public IActionResult DeleteCustomTextMaterialProp(int materialId, Guid propId)
        {
            try
            {
                // Get custom prop and validate type
                CustomMaterialProp prop = CustomMaterialPropService.GetCustomMaterialProp(propId);
                if (prop.Type != PropType.Text)
                {
                    return HandleBadRequest("The submitted prop is not of the type `text`.");
                }

                // Update material - remove prop
                MaterialsService.UpdateCustomTextMaterialProp(materialId, prop, null);

                // Done!
                return NoContent();
            }
            catch (CustomPropNotFoundException exception)
            {
                return HandleResourceNotFoundException(exception);
            }
            catch (MaterialNotFoundException exception)
            {
                return HandleResourceNotFoundException(exception);
            }
            catch (Exception exception)
            {
                return HandleUnexpectedException(exception);
            }
        }

        /// <summary>
        /// Sets a custom material property of the file type.
        /// </summary>
        /// <param name="materialId">The ID of the material to set a value for.</param>
        /// <param name="propId">The ID of the prop to set.</param>
        /// <param name="file">The file to set.</param>
        /// <returns>Returns a `204 No Content` response on success.</returns>
        [HttpPut("{materialId}/file-props/{propId}"), Authorize(Policy = AuthPolicyNames.MAY_SET_CUSTOM_MATERIAL_PROP_VALUE)]
        public IActionResult SetCustomFileMaterialProp(int materialId, Guid propId, IFormFile file)
        {
            if (file.Length <= 0)
            {
                return HandleBadRequest("No file content found.");
            }

            try
            {
                // Get custom prop and validate type
                CustomMaterialProp prop = CustomMaterialPropService.GetCustomMaterialProp(propId);
                if (prop.Type != PropType.File)
                {
                    return HandleBadRequest("The submitted prop is not of the type `file`.");
                }
                MaterialsService.UpdateCustomFileMaterialProp(materialId, prop, file);

                // Done!
                return NoContent();
            }
            catch (CustomPropNotFoundException exception)
            {
                return HandleResourceNotFoundException(exception);
            }
            catch (MaterialNotFoundException exception)
            {
                return HandleResourceNotFoundException(exception);
            }
            catch (Exception exception)
            {
                return HandleUnexpectedException(exception);
            }
        }

        /// <summary>
        /// Deletes a custom material prop value of the file type.
        /// </summary>
        /// <param name="materialId">The ID of the material.</param>
        /// <param name="propId">The ID of the prop to unset.</param>
        /// <returns>Returns a `204 No Content` response on success.</returns>
        [HttpDelete("{materialId}/file-props/{propId}"), Authorize(Policy = AuthPolicyNames.MAY_DELETE_CUSTOM_MATERIAL_PROP_VALUE)]
        public IActionResult DeleteCustomFileMaterialProp(int materialId, Guid propId)
        {
            try
            {
                // Get custom prop and validate type
                CustomMaterialProp prop = CustomMaterialPropService.GetCustomMaterialProp(propId);
                if (prop.Type != PropType.File)
                {
                    return HandleBadRequest("The submitted prop is not of the type `file`.");
                }

                // Update material - remove prop
                MaterialsService.UpdateCustomFileMaterialProp(materialId, prop, null);

                // Done!
                return NoContent();
            }
            catch (CustomPropNotFoundException exception)
            {
                return HandleResourceNotFoundException(exception);
            }
            catch (MaterialNotFoundException exception)
            {
                return HandleResourceNotFoundException(exception);
            }
            catch (Exception exception)
            {
                return HandleUnexpectedException(exception);
            }
        }

        /// <summary>
        /// Downloads a file associated with a custom material prop.
        /// </summary>
        /// <param name="materialId">The ID of the material to get the file for.</param>
        /// <param name="propId">The ID of the prop to get the file from.</param>
        /// <returns>Returns a `204 No Content` response on success.</returns>
        [HttpGet("{materialId}/file-props/{propId}/download")]
        public IActionResult DownloadCustomPropFile(int materialId, Guid propId)
        {
            try
            {
                // Get custom prop and validate type
                CustomMaterialProp prop = CustomMaterialPropService.GetCustomMaterialProp(propId);
                if (prop.Type != PropType.File)
                {
                    return HandleBadRequest("The submitted prop is not of the type `file`.");
                }

                // Get and validate file path
                string path = MaterialsService.GetCustomPropFilePath(materialId, prop);
                if (string.IsNullOrWhiteSpace(path))
                {
                    return NotFound(new ClientErrorResponse("File could not be found!"));
                }

                string fileName = Path.GetFileName(path);
                byte[] fileBytes = System.IO.File.ReadAllBytes(path);
                string contentType = "application/octet-stream";
                new FileExtensionContentTypeProvider().TryGetContentType(fileName, out contentType);
                return File(fileBytes, contentType, fileName);
            }
            catch (CustomPropNotFoundException exception)
            {
                return HandleResourceNotFoundException(exception);
            }
            catch (MaterialNotFoundException exception)
            {
                return HandleResourceNotFoundException(exception);
            }
            catch (Exception exception)
            {
                return HandleUnexpectedException(exception);
            }
        }

        #endregion
    }
}
