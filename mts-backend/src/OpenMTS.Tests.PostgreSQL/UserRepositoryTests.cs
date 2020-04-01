using Microsoft.Extensions.Logging;
using Npgsql;
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
    public class UserRepositoryTests
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
        /// Sets up the SUT and needed services.
        /// </summary>
        public UserRepositoryTests()
        {
            Repository = new PostgreSqlUserRepository(ConfigurationProvider.GetConfiguration());
            PasswordHashingService = new PasswordHashingService(new LoggerFactory());
        }

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
    }
}
