using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OpenMTS.Authorization;
using OpenMTS.Controllers.Contracts.Requests;
using OpenMTS.Models;
using OpenMTS.Services;
using OpenMTS.Services.Exceptions;
using System;

namespace OpenMTS.Controllers
{
    /// <summary>
    /// Provides an API for getting and setting OpenMTS configuration options.
    /// </summary>
    [Route("api/configuration")]
    public class ConfigurationController : ControllerBase
    {
        /// <summary>
        /// Provides access to configuration functionality.
        /// </summary>
        private ConfigurationService ConfigurationService { get; }

        /// <summary>
        /// Provides access to custom material prop functionality.
        /// </summary>
        private CustomMaterialPropService CustomMaterialPropService { get; }

        /// <summary>
        /// Provides access to custom batch prop functionality.
        /// </summary>
        private CustomBatchPropService CustomBatchPropService { get; }

        /// <summary>
        /// Initializes the controller.
        /// </summary>
        /// <param name="loggerFactory">Factory to create loggers from.</param>
        /// <param name="configurationService">The service that provides configuration functionality.</param>
        /// <param name="customPropService">The service providing custom material prop functionality.</param>
        /// <param name="customBatchPropService">The service providing custom batch prop functionality.</param>
        public ConfigurationController(ILoggerFactory loggerFactory,
            ConfigurationService configurationService,
            CustomMaterialPropService customPropService,
            CustomBatchPropService customBatchPropService)
        {
            Logger = loggerFactory.CreateLogger<ConfigurationController>();
            ConfigurationService = configurationService;
            CustomMaterialPropService = customPropService;
            CustomBatchPropService = customBatchPropService;
        }

        /// <summary>
        /// Gets the current OpenMTS configuration.
        /// </summary>
        /// <returns>Returns the configuration.</returns>
        [HttpGet]
        public IActionResult GetConfiguration()
        {
            try
            {
                return Ok(ConfigurationService.GetConfiguration());
            }
            catch (Exception exception)
            {
                return HandleUnexpectedException(exception);
            }
        }

        /// <summary>
        /// Sets the OpenMTS configuration.
        /// </summary>
        /// <param name="configuration">Configuration to set.</param>
        /// <returns>Returns `204 No Content` on success.</returns>
        [HttpPost, Authorize(Policy = AuthPolicyNames.MAY_SET_CONFIGURATION)]
        public IActionResult SetConfiguration([FromBody] Configuration configuration)
        {
            try
            {
                ConfigurationService.SetConfiguration(configuration);
                return NoContent();
            }
            catch (Exception exception)
            {
                return HandleUnexpectedException(exception);
            }
        }

        #region Custom material properties

        /// <summary>
        /// Gets all available custom material props.
        /// </summary>
        /// <returns>Returns all props.</returns>
        [HttpGet("material-props"), Authorize]
        public IActionResult GetCustomMaterialProps()
        {
            try
            {
                return Ok(CustomMaterialPropService.GetAllCustomMaterialProps());
            }
            catch (Exception exception)
            {
                return HandleUnexpectedException(exception);
            }
        }

        /// <summary>
        /// Creates a custom material property.
        /// </summary>
        /// <param name="customMaterialPropCreationRequest">The custom material property creation request.</param>
        /// <returns>Returns a `201 Created` response on success.</returns>
        [HttpPost("material-props"), Authorize(Policy = AuthPolicyNames.MAY_CREATE_CUSTOM_MATERIAL_PROP)]
        public IActionResult CreateCustomMaterialProp([FromBody] CustomMaterialPropCreationRequest customMaterialPropCreationRequest)
        {
            if (customMaterialPropCreationRequest == null ||
                string.IsNullOrWhiteSpace(customMaterialPropCreationRequest.Name))
            {
                return HandleBadRequest("Missing prop data: a name and type are required.");
            }

            // Attempt to parse type
            PropType type;
            if (Enum.IsDefined(typeof(PropType), customMaterialPropCreationRequest.Type))
            {
                type = (PropType)customMaterialPropCreationRequest.Type;
            }
            else
            {
                return HandleBadRequest("A valid prop type needs to be provided.");
            }

            // Attempt to create
            try
            {
                CustomMaterialProp prop = CustomMaterialPropService.CreateCustomMaterialProp(customMaterialPropCreationRequest.Name, type);
                return Created(GetNewResourceUri(prop.Id), prop);
            }
            catch (Exception exception)
            {
                return HandleUnexpectedException(exception);
            }
        }

        /// <summary>
        /// Updates a custom material property.
        /// </summary>
        /// <param name="id">The ID of the prop to update.</param>
        /// <param name="customMaterialPropUpdateRequest">The custom material property update request.</param>
        /// <returns>Returns the updated prop.</returns>
        [HttpPatch("material-props/{id}"), Authorize(Policy = AuthPolicyNames.MAY_UPDATE_CUSTOM_MATERIAL_PROP)]
        public IActionResult UpdateCustomMaterialProp(Guid id, [FromBody] CustomMaterialPropUpdateRequest customMaterialPropUpdateRequest)
        {
            if (customMaterialPropUpdateRequest == null ||
                string.IsNullOrWhiteSpace(customMaterialPropUpdateRequest.Name))
            {
                return HandleBadRequest("A valid prop name needs tu be supplied.");
            }

            try
            {
                CustomMaterialProp prop = CustomMaterialPropService.UpdateCustomMaterialProp(id, customMaterialPropUpdateRequest.Name);
                return Ok(prop);
            }
            catch (CustomPropNotFoundException exception)
            {
                return HandleResourceNotFoundException(exception);
            }
            catch (Exception exception)
            {
                return HandleUnexpectedException(exception);
            }

        }

        /// <summary>
        /// Deletes the custom material property.
        /// </summary>
        /// <param name="id">The ID of the property to delete.</param>
        /// <returns>Returns a `204 No Content` response.</returns>
        [HttpDelete("material-props/{id}"),Authorize(Policy = AuthPolicyNames.MAY_DELETE_CUSTOM_MATERIAL_PROP)]
        public IActionResult DeleteCustomMaterialProp(Guid id)
        {
            CustomMaterialPropService.DeleteCustomMaterialProp(id);
            return NoContent();
        }

        #endregion

        #region Custom batch properties

        /// <summary>
        /// Gets all available custom batch props.
        /// </summary>
        /// <returns>Returns all props.</returns>
        [HttpGet("batch-props"), Authorize]
        public IActionResult GetCustomMBatchProps()
        {
            try
            {
                return Ok(CustomBatchPropService.GetAllCustomBatchProps());
            }
            catch (Exception exception)
            {
                return HandleUnexpectedException(exception);
            }
        }

        /// <summary>
        /// Creates a custom batch property.
        /// </summary>
        /// <param name="customBatchPropCreationRequest">The custom batch property creation request.</param>
        /// <returns>Returns a `201 Created` response on success.</returns>
        [HttpPost("batch-props")] // TODO: auth policy
        public IActionResult CreateCustomBatchProp([FromBody] CustomBatchPropCreationRequest customBatchPropCreationRequest)
        {
            if (customBatchPropCreationRequest == null ||
                string.IsNullOrWhiteSpace(customBatchPropCreationRequest.Name))
            {
                return HandleBadRequest("Missing prop data: a name and type are required.");
            }

            try
            {
                CustomBatchProp prop = CustomBatchPropService.CreateCustomBatchProp(customBatchPropCreationRequest.Name);
                return Created(GetNewResourceUri(prop.Id), prop);
            }
            catch (Exception exception)
            {
                return HandleUnexpectedException(exception);
            }
        }

        /// <summary>
        /// Updates a custom batch property.
        /// </summary>
        /// <param name="id">The ID of the prop to update.</param>
        /// <param name="customBatchPropUpdateRequest">The custom batch property update request.</param>
        /// <returns>Returns the updated prop.</returns>
        [HttpPatch("batch-props/{id}")] // TODO: auth policy
        public IActionResult UpdateCustomBatchProp(Guid id, [FromBody] CustomBatchPropUpdateRequest customBatchPropUpdateRequest)
        {
            if (customBatchPropUpdateRequest == null ||
                string.IsNullOrWhiteSpace(customBatchPropUpdateRequest.Name))
            {
                return HandleBadRequest("A valid prop name needs tu be supplied.");
            }

            try
            {
                CustomBatchProp prop = CustomBatchPropService.UpdateCustomBatchProp(id, customBatchPropUpdateRequest.Name);
                return Ok(prop);
            }
            catch (CustomPropNotFoundException exception)
            {
                return HandleResourceNotFoundException(exception);
            }
            catch (Exception exception)
            {
                return HandleUnexpectedException(exception);
            }

        }

        /// <summary>
        /// Deletes the custom batch property.
        /// </summary>
        /// <param name="id">The ID of the property to delete.</param>
        /// <returns>Returns a `204 No Content` response.</returns>
        [HttpDelete("batch-props/{id}")] // TODO: auth policy
        public IActionResult DeleteCustomBatchProp(Guid id)
        {
            CustomBatchPropService.DeleteCustomBatchProp(id);
            return NoContent();
        }

        #endregion
    }
}
