﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OpenMTS.Authorization;
using OpenMTS.Controllers.Contracts.Requests;
using OpenMTS.Models;
using OpenMTS.Services;
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
        private CustomMaterialPropService CustomPropService { get; }

        /// <summary>
        /// Initializes the controller.
        /// </summary>
        /// <param name="loggerFactory">Factory to create loggers from.</param>
        /// <param name="configurationService">The service that provides configuration functionality.</param>
        /// <param name="customPropService">The service providing custom prop functionality.</param>
        public ConfigurationController(ILoggerFactory loggerFactory,
            ConfigurationService configurationService,
            CustomMaterialPropService customPropService)
        {
            Logger = loggerFactory.CreateLogger<ConfigurationController>();
            ConfigurationService = configurationService;
            CustomPropService = customPropService;
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
        [HttpGet("material-props")]
        public IActionResult GetCustomMaterialProps()
        {
            try
            {
                return Ok(CustomPropService.GetAllCustomMaterialProps());
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
        [HttpPost("material-props")] // TODO - auth policy
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
                CustomMaterialProp prop = CustomPropService.CreateCustomMaterialProp(customMaterialPropCreationRequest.Name, type);
                return Created(GetNewResourceUri(prop.Id), prop);
            }
            catch (Exception exception)
            {
                return HandleUnexpectedException(exception);
            }
        }

        #endregion
    }
}
