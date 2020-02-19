using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OpenMTS.Controllers.Contracts.Responses;
using OpenMTS.Services;
using OpenMTS.Services.Authentication;
using System;
using System.IO;

namespace OpenMTS.Controllers
{
    /// <summary>
    /// API route for authentication.
    /// </summary>
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        /// <summary>
        /// The service providing OpenMTS configuration functionality.
        /// </summary>
        private ConfigurationService ConfigurationService { get; }

        /// <summary>
        /// The service providing authentication functionality.
        /// </summary>
        private AuthService AuthService { get; }

        /// <summary>
        /// Initializes a controller instance.
        /// </summary>
        /// <param name="loggerFactory">Factory to create loggers from.</param>
        /// <param name="configurationService">Service for configuration access.</param>
        /// <param name="authService">Injected authentication service.</param>
        public AuthController(ILoggerFactory loggerFactory, ConfigurationService configurationService, AuthService authService)
        {
            Logger = loggerFactory.CreateLogger<AuthController>();
            ConfigurationService = configurationService;
            AuthService = authService;
        }

        /// <summary>
        /// Attempts to authenticates a client with one of the available authentication methods.
        /// </summary>
        /// <param name="method">The authentication method to use.</param>
        /// <returns>Returns the token in successful cases.</returns>
        [HttpPost]
        public IActionResult Authenticate([FromQuery] string method)
        {
            if (string.IsNullOrWhiteSpace(method))
            {
                return HandleBadRequest("No authentication method supplied");
            }

            // Parse authentication method
            AuthenticationMethod authenticationMethod;
            try
            {
                authenticationMethod = Enum.Parse<AuthenticationMethod>(method, true);
            }
            catch (Exception)
            {
                return HandleBadRequest("Unknown authentication method.");
            }

            // Check whether guest login is allowed
            if (authenticationMethod == AuthenticationMethod.GuestLogin && !ConfigurationService.GetConfiguration().AllowGuestLogin)
            {
                return Unauthorized();
            }

            // Parse body
            string body = null;
            using (StreamReader reader = new StreamReader(Request.Body))
            {
                body = reader.ReadToEnd();
            }
            if (string.IsNullOrWhiteSpace(body))
            {
                return HandleBadRequest("Empty of invalid request body.");
            }

            // Attempt ot authenticate
            try
            {
                if (AuthService.TryAuthenticate(authenticationMethod, body, out string token))
                {
                    return Ok(new AuthResponse(token));
                }
                else
                {
                    return Unauthorized();
                }
            }
            catch (MalformedAuthenticationDataException)
            {
                return HandleBadRequest("Malformed authentication data was sent.");
            }
        }
    }
}
