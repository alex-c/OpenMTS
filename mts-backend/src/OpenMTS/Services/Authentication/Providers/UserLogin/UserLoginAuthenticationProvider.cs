using OpenMTS.Models;
using OpenMTS.Repositories;
using System.Collections.Generic;

namespace OpenMTS.Services.Authentication.Providers.UserLogin
{
    public class UserLoginAuthenticationProvider : AuthenticationProviderBase, IAuthenticationProvider
    {
        /// <summary>
        /// Provides password hashing functionalities.
        /// </summary>
        private PasswordHashingService PasswordHashingService { get; }

        /// <summary>
        /// Grants access to user information.
        /// </summary>
        private IReadOnlyUserRepository UserRepository { get; }

        /// <summary>
        /// Indicates which authentication method this provider is for.
        /// </summary>
        public AuthenticationMethod AuthenticationMethod { get; } = AuthenticationMethod.UserLogin;

        /// <summary>
        /// Sets up this provider for user login authentication.
        /// </summary>
        /// <param name="passwordHashingService">Provides hashing functionality.</param>
        /// <param name="userRepository">User repository for access to user data.</param>
        public UserLoginAuthenticationProvider(PasswordHashingService passwordHashingService, IReadOnlyUserRepository userRepository)
        {
            PasswordHashingService = passwordHashingService;
            UserRepository = userRepository;
        }

        /// <summary>
        /// Attempts to authenticate a user with his unique ID and password.
        /// </summary>
        /// <param name="data">User data as a JSON string.</param>
        /// <param name="subject">The ID user.</param>
        /// <param name="roles">Roles of the successfully authenticated user.</param>
        /// <param name="rights">Rights of the successfully authenticated user.</param>
        /// <returns>Returns whether the user was successfully authenticated.</returns>
        public bool TryAuthenticate(string data, out string subject, out IEnumerable<Role> roles, out IEnumerable<Right> rights)
        {
            UserLoginData userLoginData = ParseData<UserLoginData>(data);

            // Initialize out-parameters
            subject = userLoginData.Id;
            roles = null;
            rights = new List<Right>();

            // Look up user
            User user = UserRepository.GetUser(subject);
            if (user == null)
            {
                return false;
            }

            // Check password
            if (PasswordHashingService.HashAndSaltPassword(userLoginData.Password, user.Salt) != user.Password)
            {
                return false;
            }

            // Set roles and return!
            roles = new List<Role>() { user.Role };
            return true;
        }
    }
}
