﻿using Microsoft.AspNetCore.Authorization;
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
                string.IsNullOrWhiteSpace(userCreationRequest.Password) ||
                string.IsNullOrWhiteSpace(userCreationRequest.Role))
            {
                return HandleBadRequest("A valid user ID, name, password and role need to be provided.");
            }

            if (!Enum.TryParse(userCreationRequest.Role, out Role role))
            {
                return HandleBadRequest("A valid user role needs to be provided.");
            }

            try
            {
                User user = UserService.CreateUser(userCreationRequest.Id, userCreationRequest.Name, userCreationRequest.Password, role);
                return Created(GetNewResourceUri(user.Id), new UserResponse(user));
            }
            catch (UserAlreadyExistsException exception)
            {
                return HandleResourceAlreadyExistsException(exception);
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
            if (userUpdateRequest == null ||
                string.IsNullOrWhiteSpace(userUpdateRequest.Name) ||
                string.IsNullOrWhiteSpace(userUpdateRequest.Role))
            {
                return HandleBadRequest("A valid user name and role need to be provided.");
            }

            if (!Enum.TryParse(userUpdateRequest.Role, out Role role))
            {
                return HandleBadRequest("A valid user role needs to be provided.");
            }

            try
            {
                User user = UserService.UpdateUser(id, userUpdateRequest.Name, role);
                return Ok(new UserResponse(user));
            }
            catch (UserNotFoundException exception)
            {
                return HandleResourceNotFoundException(exception);
            }
        }

        #endregion
    }
}
