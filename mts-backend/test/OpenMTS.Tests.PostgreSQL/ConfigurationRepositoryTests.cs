using Microsoft.Extensions.Logging.Abstractions;
using OpenMTS.Models;
using OpenMTS.Repositories;
using OpenMTS.Repositories.PostgreSQL;
using System;
using Xunit;

namespace OpenMTS.Tests.PostgreSQL
{
    /// <summary>
    /// Tests for the PostgreSQL-implementation of the configuration repository.
    /// </summary>
    public class ConfigurationRepositoryTests : IDisposable
    {
        /// <summary>
        /// The SUT: a configuration repositoy.
        /// </summary>
        private IConfigurationRepository Repository { get; }

        /// <summary>
        /// Sets up the SUT and resets configuration.
        /// </summary>
        public ConfigurationRepositoryTests()
        {
            Repository = new PostgreSqlConfigurationRepository(ConfigurationProvider.GetConfiguration(), new NullLogger<PostgreSqlConfigurationRepository>());
            DatabasePurger.ResetConfiguration();
        }

        /// <summary>
        /// Tests the setting and getting of the guest login configuration.
        /// </summary>
        [Fact]
        public void TestGuestLoginConfiguration()
        {
            Configuration configuration = Repository.GetConfiguration();
            Assert.False(configuration.AllowGuestLogin);
            configuration.AllowGuestLogin = true;
            Repository.SetConfiguration(configuration);
            configuration = Repository.GetConfiguration();
            Assert.True(configuration.AllowGuestLogin);
        }

        /// <summary>
        /// Resets the OpenMTS configuration to default values.
        /// </summary>
        public void Dispose()
        {
            DatabasePurger.ResetConfiguration();
        }
    }
}
