using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using OpenMTS.Models;
using OpenMTS.Repositories;
using OpenMTS.Repositories.Memory;
using OpenMTS.Repositories.PostgreSQL;
using OpenMTS.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace OpenMTS.Tests.PostgreSQL
{
    /// <summary>
    /// Tests for the PostgreSQL-implementation of the API key repository.
    /// </summary>
    public class ApiKeyRepositoryTests : IDisposable
    {
        /// <summary>
        /// The SUT: an API key repositoy.
        /// </summary>
        private IApiKeyRepository Repository { get; }

        /// <summary>
        /// Provides rights.
        /// </summary>
        private RightsService RightsService { get; }

        /// <summary>
        /// Sets up the SUT and needed services and and cleans up API key tbales.
        /// </summary>
        public ApiKeyRepositoryTests()
        {
            Repository = new PostgreSqlApiKeyRepository(ConfigurationProvider.GetConfiguration(), new NullLogger<PostgreSqlApiKeyRepository>());
            RightsService = new RightsService(new LoggerFactory(), new MemoryRightsRepository());
            DatabasePurger.PurgeApiKeys();
        }

        /// <summary>
        /// Tests the creation of API keys.
        /// </summary>
        [Fact]
        public void TestApiKeyCreation()
        {
            Assert.Empty(Repository.GetAllApiKeys());

            // Test creation
            string name = "Test Key";
            ApiKey key = Repository.CreateApiKey(name);
            Assert.Equal(name, key.Name);
            Assert.Empty(key.Rights);
            Assert.False(key.Enabled);

            // Test getting the key from the repo again
            Assert.Single(Repository.GetAllApiKeys());
            key = Repository.GetApiKey(key.Id);
            Assert.Equal(name, key.Name);
            Assert.Empty(key.Rights);
            Assert.False(key.Enabled);

            // Create another key
            Repository.CreateApiKey("RandomName");
            Assert.Equal(2, Repository.GetAllApiKeys().Count());
        }

        /// <summary>
        /// Tests the updating of API keys.
        /// </summary>
        [Fact]
        public void TestApiKeyUpdate()
        {
            Assert.Empty(Repository.GetAllApiKeys());

            // Test creation
            string name = "Test Key";
            ApiKey key = Repository.CreateApiKey(name);
            Assert.Equal(name, key.Name);
            Assert.Empty(key.Rights);
            Assert.False(key.Enabled);

            // Update key, adding rights and changing name
            name = "Aloha";
            key.Name = name;
            key.Rights = new List<Right>()
            {
                new Right(RightIds.BATCHES_PERFORM_TRANSACTION),
                new Right(RightIds.BATCHES_UPDATE_STATUS),
                new Right(RightIds.BATCHES_UPDATE)
            };
            Repository.UpdateApiKey(key);

            // Get key again and check rights
            key = Repository.GetApiKey(key.Id);
            Assert.Equal(name, key.Name);
            Assert.Equal(3, key.Rights.Count);
            Assert.Contains(key.Rights, r => r.Id == RightIds.BATCHES_PERFORM_TRANSACTION);
            Assert.Contains(key.Rights, r => r.Id == RightIds.BATCHES_UPDATE_STATUS);
            Assert.Contains(key.Rights, r => r.Id == RightIds.BATCHES_UPDATE);

            // Update again, removing rights
            key.Rights = new List<Right>() { new Right(RightIds.BATCHES_PERFORM_TRANSACTION) };
            Repository.UpdateApiKey(key);

            // Get key again and check rights
            key = Repository.GetApiKey(key.Id);
            Assert.Equal(name, key.Name);
            Assert.Equal(1, key.Rights.Count);
            Assert.Contains(key.Rights, r => r.Id == RightIds.BATCHES_PERFORM_TRANSACTION);

            // Check key count
            Assert.Single(Repository.GetAllApiKeys());

            // Add and update a second key
            key = Repository.CreateApiKey("SomeName");
            key.Rights = new List<Right>()
            {
                new Right(RightIds.CUSTOM_BATCH_PROPS_CREATE),
                new Right(RightIds.CUSTOM_BATCH_PROPS_UPDATE),
                new Right(RightIds.CUSTOM_BATCH_PROPS_DELETE)
            };
            Repository.UpdateApiKey(key);

            // Check keys
            IEnumerable<ApiKey> keys = Repository.GetAllApiKeys();
            Assert.Equal(2, keys.Count());
            Assert.Contains(keys, k => k.Name == "Aloha" && k.Rights.Count == 1);
            Assert.Contains(keys, k => k.Name == "SomeName" && k.Rights.Count == 3);
        }

        /// <summary>
        /// Purges API key tables.
        /// </summary>
        public void Dispose()
        {
            DatabasePurger.PurgeApiKeys();
        }
    }
}
