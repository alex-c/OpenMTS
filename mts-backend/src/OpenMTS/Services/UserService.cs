using OpenMTS.Models;
using OpenMTS.Repositories;
using OpenMTS.Services.Exceptions;
using System;
using System.Collections.Generic;

namespace OpenMTS.Services
{
    public class UserService
    {
        /// <summary>
        /// Provides password hashing features.
        /// </summary>
        private PasswordHashingService PasswordHashingService { get; }

        /// <summary>
        /// Provides access to user data persistence.
        /// </summary>
        private IUserRepository UserRepository { get; }

        /// <summary>
        /// Sets up the service.
        /// </summary>
        /// <param name="passwordHashingService">Provides hashing features.</param>
        /// <param name="userRepository">Repository for user data.</param>
        public UserService(PasswordHashingService passwordHashingService, IUserRepository userRepository)
        {
            PasswordHashingService = passwordHashingService;
            UserRepository = userRepository;
        }

        /// <summary>
        /// Gets all available users.
        /// </summary>
        /// <param name="showDisabled">Whether to return disabled users.</param>
        /// <returns>Returns a list of users.</returns>
        public IEnumerable<User> GetAllUsers(bool showDisabled = false)
        {
            return UserRepository.GetAllUsers(showDisabled);
        }

        /// <summary>
        /// Searches users by name using a partial name.
        /// </summary>
        /// <param name="partialName">Partial name to search for.</param>
        /// <param name="showDisabled">Whether to return disabled users.</param>
        /// <returns>Returns a list of matching users.</returns>
        public IEnumerable<User> SearchUsersByName(string partialName, bool showDisabled = false)
        {
            return UserRepository.SearchUsersByName(partialName, showDisabled);
        }

        /// <summary>
        /// Gets a user by his unique id.
        /// </summary>
        /// <param name="id">Id of the user to get.</param>
        /// <returns>Returns the user if found.</returns>
        /// <exception cref="UserNotFoundException">Thrown if there is no user with the given id.</exception>
        public User GetUser(string id)
        {
            return GetUserOrThrowNotFoundException(id);
        }
        
        /// <summary>
        /// Creates a new user.
        /// </summary>
        /// <param name="id">An ID for the user to create.</param>
        /// <param name="name">A name for the user to create.</param>
        /// <param name="password">A password for the user.</param>
        /// <param name="role">A user role to assign to the user.</param>
        /// <returns>Returns the newly created user.</returns>
        /// <exception cref="UserAlreadyExistsException">Thrown if the login name is already taken.</exception>
        public User CreateUser(string id, string name, string password, Role role)
        {
            if (UserRepository.GetUser(id) != null)
            {
                throw new UserAlreadyExistsException(id);
            }

            // Hash & salt password, create user!
            (string hash, byte[] salt) = PasswordHashingService.HashAndSaltPassword(password);
            return UserRepository.CreateUser(id, name, hash, salt, role);
        }

        /// <summary>
        /// Updates an existing user.
        /// </summary>
        /// <param name="id">The ID of the user to update.</param>
        /// <param name="name">The new user name to set.</param>
        /// <param name="role">The user role to assign to the user.</param>
        /// <returns>Returns the modified user model.</returns>
        /// <exception cref="UserNotFoundException">Thrown if there is no such user to update.</exception>
        public User UpdateUser(string id, string name, Role role)
        {
            User user = GetUserOrThrowNotFoundException(id);
            
            user.Name = name;
            user.Role = role;
            UserRepository.UpdateUser(user);

            return user;
        }

        /// <summary>
        /// Updates a user's status: whether he is disabled or not.
        /// </summary>
        /// <param name="id">ID of the user to update.</param>
        /// <param name="isDisabled">Whether the user is disabled.</param>
        public void UpdateUserStatus(string id, bool isDisabled)
        {
            User user = GetUserOrThrowNotFoundException(id);
            user.Disabled = isDisabled;
            UserRepository.UpdateUser(user);
        }

        /// <summary>
        /// Attempts to change a user's password
        /// </summary>
        /// <param name="id">The ID of the user.</param>
        /// <param name="oldPassword">The old password for verification.</param>
        /// <param name="newPassword">The new password to save.</param>
        /// <exception cref="UserNotFoundException">Thrown if there is no such user.</exception>
        /// <exception cref="UnauthorizedAccessException">Thrown if the submitted old password is wrong!</exception>
        public void ChangePassword(string id, string oldPassword, string newPassword)
        {
            User user = GetUserOrThrowNotFoundException(id);

            // Verify old password
            if (user.Password != PasswordHashingService.HashAndSaltPassword(oldPassword, user.Salt))
            {
                throw new UnauthorizedAccessException();
            }

            // Hash and salt new password
            (string hashedPassword, byte[] salt) = PasswordHashingService.HashAndSaltPassword(newPassword);
            user.Password = hashedPassword;
            user.Salt = salt;
            UserRepository.UpdateUser(user);
        }

        #region Private Helpers

        /// <summary>
        /// Attempts to get a user from the underlying repository and throws a <see cref="UserNotFoundException"/> if no matching user could be found.
        /// </summary>
        /// <param name="id">ID of the user to get.</param>
        /// <exception cref="UserNotFoundException">Thrown if no matching user could be found.</exception>
        /// <returns>Returns the user, if found.</returns>
        private User GetUserOrThrowNotFoundException(string id)
        {
            User user = UserRepository.GetUser(id);

            // Check for user existence
            if (user == null)
            {
                throw new UserNotFoundException(id);
            }

            return user;
        }

        #endregion
    }
}
