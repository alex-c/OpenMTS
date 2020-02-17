using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
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
    /// API route for user data.
    /// </summary>
    [Route("api/users")]
    public class UserController : ControllerBase
    {
        /// <summary>
        /// The service providing user-related CRUD functionality.
        /// </summary>
        private UserService UserService { get; }

        /// <summary>
        /// Initializes the controller with all needed components.
        /// </summary>
        /// <param name="loggerFactory">Factory to create loggers from.</param>
        /// <param name="userService">Service providing user-related functionality.</param>
        public UserController(ILoggerFactory loggerFactory, UserService userService)
        {
            Logger = loggerFactory.CreateLogger<UserController>();
            UserService = userService;
        }

        #region Public getters

        /// <summary>
        /// Gets all available users.
        /// </summary>
        /// <returns>Returns a list of users.</returns>
        [HttpGet]
        public IActionResult GetAllUsers()
        {
            try
            {
                IEnumerable<User> users = UserService.GetAllUsers();
                return Ok(users.Select(u => new UserResponse(u)));
            }
            catch (Exception exception)
            {
                return HandleUnexpectedException(exception);
            }
        }

        /// <summary>
        /// Gets a specific user by his unique ID.
        /// </summary>
        /// <param name="id">ID of the user to get.</param>
        /// <returns>Returns the user, if found.</returns>
        [HttpGet("{id}")]
        public IActionResult GetUser(string id)
        {
            try
            {
                User user = UserService.GetUser(id);
                return Ok(new UserResponse(user));
            }
            catch (UserNotFoundException exception)
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
