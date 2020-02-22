using Microsoft.AspNetCore.Authorization;
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
        /// Gets available users.
        /// </summary>
        /// <param name="page">Requested page of data.</param>
        /// <param name="elementsPerPage">Number of elements requested.</param>
        /// <param name="search">Optional partial name to filter users with.</param>
        /// <param name="showDisabled">Whether to show disabled users.</param>
        /// <returns>Returns a paginated list of users.</returns>
        [HttpGet]
        public IActionResult GetUsers([FromQuery] int page = 1, [FromQuery] int elementsPerPage = 10, [FromQuery] string search = null, [FromQuery] bool showDisabled = false)
        {
            try
            {
                IEnumerable<User> users = null;
                if (string.IsNullOrWhiteSpace(search))
                {
                    users = UserService.GetAllUsers(showDisabled);
                }
                else
                {
                    users = UserService.SearchUsersByName(search, showDisabled);
                }
                IEnumerable<User> paginatedUsers = users.Skip((page - 1) * elementsPerPage).Take(elementsPerPage);
                return Ok(new PaginatedResponse(paginatedUsers.Select(u => new UserResponse(u)), users.Count()));
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

        #region Administrative features

        /// <summary>
        /// Creates a new user.
        /// </summary>
        /// <param name="userCreationRequest">The request contract.</param>
        /// <returns>Returns the newly created user.</returns>
        [HttpPost, Authorize(Roles = "Administrator")]
        public IActionResult CreateUser([FromBody] UserCreationRequest userCreationRequest)
        {
            if (userCreationRequest == null ||
                string.IsNullOrWhiteSpace(userCreationRequest.Id) ||
                string.IsNullOrWhiteSpace(userCreationRequest.Name) ||
                string.IsNullOrWhiteSpace(userCreationRequest.Password))
            {
                return HandleBadRequest("A valid user ID, name, password and role need to be provided.");
            }

            // Special check to avoid conflicts with guest login functionality
            if (userCreationRequest.Id == "openmts.guest")
            {
                return Conflict(new ClientErrorResponse("The ID `openmts.guest` is not available."));
            }

            // Attempt to parse role
            Role role;
            if (Enum.IsDefined(typeof(Role), userCreationRequest.Role))
            {
                role = (Role)userCreationRequest.Role;
            }
            else
            {
                return HandleBadRequest("A valid user role needs to be provided.");
            }

            // Attempt to create the new user
            try
            {
                User user = UserService.CreateUser(userCreationRequest.Id, userCreationRequest.Name, userCreationRequest.Password, role);
                return Created(GetNewResourceUri(user.Id), new UserResponse(user));
            }
            catch (UserAlreadyExistsException exception)
            {
                return HandleResourceAlreadyExistsException(exception);
            }
            catch (Exception exception)
            {
                return HandleUnexpectedException(exception);
            }
        }

        /// <summary>
        /// Updates an existing user's data.
        /// </summary>
        /// <param name="id">If of the user to update.</param>
        /// <param name="userUpdateRequest">Request contract with data to update.</param>
        /// <returns>Returns the modified user.</returns>
        [HttpPatch("{id}"), Authorize(Roles = "Administrator")]
        public IActionResult UpdateUser(string id, [FromBody] UserUpdateRequest userUpdateRequest)
        {
            if (userUpdateRequest == null || string.IsNullOrWhiteSpace(userUpdateRequest.Name))
            {
                return HandleBadRequest("A valid user name and role need to be provided.");
            }

            // Attempt to parse role
            Role role;
            if (Enum.IsDefined(typeof(Role), userUpdateRequest.Role))
            {
                role = (Role)userUpdateRequest.Role;
            }
            else
            {
                return HandleBadRequest("A valid user role needs to be provided.");
            }

            // Attempt to update the user
            try
            {
                User user = UserService.UpdateUser(id, userUpdateRequest.Name, role);
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

        /// <summary>
        /// Allows to update a user's status (whether he is disabled or not).
        /// </summary>
        /// <param name="id">Id of the user to update.</param>
        /// <param name="updateUserStatusRequest">The request contract containing whether the user should be disabled or not.</param>
        /// <returns>Returns `204 No Content` on success.</returns>
        [HttpPut("{id}/status"), Authorize(Roles = "Administrator")]
        public IActionResult UpdateUserStatus(string id, [FromBody] UpdateUserStatusRequest updateUserStatusRequest)
        {
            if (updateUserStatusRequest == null)
            {
                return HandleBadRequest("Missing status data.");
            }

            try
            {
                UserService.UpdateUserStatus(id, updateUserStatusRequest.Disabled);
                return NoContent();
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
