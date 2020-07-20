using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using OpenMTS.Models;
using OpenMTS.Repositories;
using OpenMTS.Repositories.PostgreSQL;
using OpenMTS.Services;
using System;
using System.Linq;
using Xunit;

namespace OpenMTS.Tests.PostgreSQL
{
    /// <summary>
    /// Tests for the PostgreSQL-implementation of the user repository.
    /// </summary>
    public class UserRepositoryTests : IDisposable
    {
        /// <summary>
        /// The SUT: a user repositoy.
        /// </summary>
        private IUserRepository Repository { get; }

        /// <summary>
        /// Provides password hashing functionality.
        /// </summary>
        private PasswordHashingService PasswordHashingService { get; }

        /// <summary>
        /// Sets up the SUT and needed services, and cleans up the users table.
        /// </summary>
        public UserRepositoryTests()
        {
            Repository = new PostgreSqlUserRepository(ConfigurationProvider.GetConfiguration(), new NullLogger<PostgreSqlUserRepository>());
            PasswordHashingService = new PasswordHashingService(new LoggerFactory());
            DatabasePurger.PurgeUsers();
        }

        /// <summary>
        /// Tests the creation of users.
        /// </summary>
        /// <param name="role">Role of the user to create.</param>
        [Theory]
        [InlineData(Role.Administrator)]
        [InlineData(Role.ScientificAssistant)]
        [InlineData(Role.User)]
        public void TestUserCreation(Role role)
        {
            int oldUserCount = Repository.GetAllUsers().Count();

            // Create user
            (string password, byte[] salt) = PasswordHashingService.HashAndSaltPassword(Guid.NewGuid().ToString());
            string id = Guid.NewGuid().ToString().Substring(0, 32);
            string name = Guid.NewGuid().ToString();
            Repository.CreateUser(id, name, password, salt, role);

            // Test insertion
            Assert.Equal(oldUserCount + 1, Repository.GetAllUsers().Count());

            // Test user
            User user = Repository.GetUser(id);
            Assert.Equal(id, user.Id);
            Assert.Equal(name, user.Name);
            Assert.Equal(password, user.Password);
            Assert.Equal(salt, user.Salt);
            Assert.Equal(role, user.Role);
            Assert.False(user.Disabled);
        }

        /// <summary>
        /// Tests the updating of an existing user.
        /// </summary>
        [Fact]
        public void TestUserUpdate()
        {
            // Create user to test
            (string password, byte[] salt) = PasswordHashingService.HashAndSaltPassword(Guid.NewGuid().ToString());
            string id = Guid.NewGuid().ToString().Substring(0, 32);
            string name = Guid.NewGuid().ToString();
            Repository.CreateUser(id, name, password, salt, Role.User);

            // Get user count
            int oldUserCount = Repository.GetAllUsers().Count();

            // Update user role and name
            User user = Repository.GetUser(id);
            user.Role = Role.ScientificAssistant;
            user.Name = "Test Name";
            Repository.UpdateUser(user);

            // Test updated data
            Assert.Equal(oldUserCount, Repository.GetAllUsers().Count());
            user = Repository.GetUser(id);
            Assert.Equal(Role.ScientificAssistant, user.Role);
            Assert.Equal("Test Name", user.Name);
        }

        /// <summary>
        /// Tests the disabling of users.
        /// </summary>
        [Fact]
        public void TestUserDisabling()
        {
            // Create user to test
            (string password, byte[] salt) = PasswordHashingService.HashAndSaltPassword(Guid.NewGuid().ToString());
            string id = Guid.NewGuid().ToString().Substring(0, 32);
            string name = Guid.NewGuid().ToString();
            Repository.CreateUser(id, name, password, salt, Role.User);

            // Get user count
            int oldUserCount = Repository.GetAllUsers().Count();
            int oldDisabledCount = Repository.GetAllUsers(true).Count();

            // Disable user
            User user = Repository.GetUser(id);
            user.Disabled = true;
            Repository.UpdateUser(user);

            var u1 = Repository.GetAllUsers();
            var u2 = Repository.GetAllUsers(true);

            // Test updated data
            Assert.Equal(oldUserCount - 1, Repository.GetAllUsers().Count());
            Assert.Equal(oldDisabledCount, Repository.GetAllUsers(true).Count());
            user = Repository.GetUser(id);
            Assert.True(user.Disabled);
        }

        /// <summary>
        /// Tests the deletion of users.
        /// </summary>
        [Fact]
        public void TestUserDeletion()
        {
            // Create user to test
            (string password, byte[] salt) = PasswordHashingService.HashAndSaltPassword(Guid.NewGuid().ToString());
            string id = Guid.NewGuid().ToString().Substring(0, 32);
            string name = Guid.NewGuid().ToString();
            Repository.CreateUser(id, name, password, salt, Role.User);

            // Get user count
            int oldUserCount = Repository.GetAllUsers().Count();

            // Delete user
            Repository.DeleteUser(id);

            // Test deletion
            Assert.Equal(oldUserCount - 1, Repository.GetAllUsers().Count());
            Assert.Null(Repository.GetUser(id));
        }

        /// <summary>
        /// Tests filtering users using a filter for the name.
        /// </summary>
        [Fact]
        public void TestUserSearchByName()
        {
            // Create users to test
            (string password, byte[] salt) = PasswordHashingService.HashAndSaltPassword(Guid.NewGuid().ToString());
            User alex = Repository.CreateUser(Guid.NewGuid().ToString().Substring(0, 5) + "alex", "Anna Pilot", password, salt, Role.User);
            User anna = Repository.CreateUser(Guid.NewGuid().ToString().Substring(0, 5) + "anna", "Alex Annamann", password, salt, Role.User);
            User jinx = Repository.CreateUser(Guid.NewGuid().ToString().Substring(0, 5) + "jinx", "Jinx Pilot", password, salt, Role.User);

            // Search
            Assert.Single(Repository.SearchUsersByName("jinx"));
            Assert.Single(Repository.SearchUsersByName("alex"));
            Assert.Equal(2, Repository.SearchUsersByName("anna").Count());
            Assert.Equal(2, Repository.SearchUsersByName("pilot").Count());
        }

        /// <summary>
        /// Cleans up the users table!
        /// </summary>
        public void Dispose()
        {
            DatabasePurger.PurgeUsers();
        }
    }
}
