using OpenMTS.Models;
using OpenMTS.Repositories;
using OpenMTS.Services.Exceptions;
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
        /// <returns>Returns a list of users.</returns>
        public IEnumerable<User> GetAllUsers()
        {
            return UserRepository.GetAllUsers();
        }

        /// <summary>
        /// Gets a user by his unique id.
        /// </summary>
        /// <param name="id">Id of the user to get.</param>
        /// <returns>Returns the user if found.</returns>
        /// <exception cref="UserNotFoundException">Thrown if there is no user with the given id.</exception>
        public User GetUser(string id)
        {
            User user = UserRepository.GetUser(id);
            if (user == null)
            {
                throw new UserNotFoundException(id);
            }
            return user;
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
    }
}
