using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
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
        /// Initializes the controller.
        /// </summary>
        /// <param name="loggerFactory">Factory to create loggers from.</param>
        /// <param name="configurationService">The service that provides configuration functionality.</param>
        public ConfigurationController(ILoggerFactory loggerFactory, ConfigurationService configurationService)
        {
            Logger = loggerFactory.CreateLogger<ConfigurationController>();
            ConfigurationService = configurationService;
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
        [HttpPost, Authorize(Roles = "Administrator")]
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
    }
}
