using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using OpenMTS.Models;
using OpenMTS.Repositories;
using OpenMTS.Repositories.PostgreSQL;
using OpenMTS.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace OpenMTS.Tests.PostgreSQL
{
    /// <summary>
    /// Tests the PostgreSQL implementation of the transactions repository.
    /// </summary>
    /// <seealso cref="System.IDisposable" />
    public class TransactionRepositoryTests : IDisposable
    {
        /// <summary>
        /// The SUT: a transaction repository.
        /// </summary>
        private ITransactionRepository Repository { get; }

        // Repositories needed to create prerequisite entities
        private IMaterialBatchRepository BatchRepository { get; }
        private ILocationsRepository LocationsRepository { get; }
        private IMaterialsRepository MaterialsRepository { get; }
        private IPlasticsRepository PlasticsRepository { get; }
        private IUserRepository UserRepository { get; }

        /// <summary>
        /// Sets up all needed repositories and purges all related DB tables.
        /// </summary>
        public TransactionRepositoryTests()
        {
            IConfiguration configuration = ConfigurationProvider.GetConfiguration();
            Repository = new PostgreSqlTransactionRepository(configuration, new NullLogger<PostgreSqlTransactionRepository>());
            BatchRepository = new PostgreSqlMaterialBatchRepository(configuration, new NullLogger<PostgreSqlMaterialBatchRepository>());
            LocationsRepository = new PostgreSqlLocationsRepository(configuration, new NullLogger<PostgreSqlLocationsRepository>());
            MaterialsRepository = new PostgreSqlMaterialsRepository(configuration, new NullLogger<PostgreSqlMaterialsRepository>());
            PlasticsRepository = new PostgreSqlPlasticsRepository(configuration, new NullLogger<PostgreSqlPlasticsRepository>());
            UserRepository = new PostgreSqlUserRepository(configuration, new NullLogger<PostgreSqlUserRepository>());
            Dispose();
        }

        /// <summary>
        /// Tests all methods of the transaction repository.
        /// </summary>
        [Fact]
        public void TestTransactionRepository()
        {
            // Create prerequisite entities
            Plastic plastic = PlasticsRepository.CreatePlastic("PP", "Polypropylene");
            Material material = MaterialsRepository.CreateMaterial("m", "m", "m", plastic);
            StorageSite site = LocationsRepository.CreateStorageSite("site");
            StorageArea area = LocationsRepository.CreateStorageArea(site, "area");
            StorageLocation location = new StorageLocation()
            {
                StorageSiteId = site.Id,
                StorageAreaId = area.Id,
                StorageSiteName = site.Name,
                StorageAreaName = area.Name
            };
            MaterialBatch batch = BatchRepository.CreateMaterialBatch(material, DateTime.Today.AddDays(3), location, 42, 42, new Dictionary<Guid, string>(), false);
            (string password, byte[] salt) = new PasswordHashingService(new LoggerFactory()).HashAndSaltPassword("test");
            User user = UserRepository.CreateUser("alex", "Alex", password, salt, Role.Administrator);

            // Create transactions
            Repository.LogTransaction(new Transaction()
            {
                Id = Guid.NewGuid(),
                MaterialBatchId = batch.Id,
                Quantity = -3,
                Timestamp = DateTime.Today.AddDays(-3),
                UserId = user.Id
            });
            Repository.LogTransaction(new Transaction()
            {
                Id = Guid.NewGuid(),
                MaterialBatchId = batch.Id,
                Quantity = -2,
                Timestamp = DateTime.Today.AddDays(-2),
                UserId = user.Id
            });
            Guid lastId = Guid.NewGuid();
            Repository.LogTransaction(new Transaction()
            {
                Id = lastId,
                MaterialBatchId = batch.Id,
                Quantity = -1,
                Timestamp = DateTime.Today.AddDays(-1),
                UserId = user.Id
            });

            // Check with getAll
            IEnumerable<Transaction> transactions = Repository.GetTransactionsForBatch(batch.Id);
            Assert.Equal(3, transactions.Count());
            Assert.Single(transactions, t => t.Quantity == -3);
            Assert.Single(transactions, t => t.Quantity == -2);
            Assert.Single(transactions, t => t.Quantity == -1);

            // Check get last
            Transaction transaction = Repository.GetLastTransactionForBatch(batch.Id);
            Assert.Equal(batch.Id, transaction.MaterialBatchId);
            Assert.Equal(lastId, transaction.Id);
            Assert.Equal(-1, transaction.Quantity);

            // Test transaction update
            transaction.Quantity = 200;
            Repository.UpdateTransaction(transaction);
            transaction = Repository.GetLastTransactionForBatch(batch.Id);
            Assert.Equal(lastId, transaction.Id);
            Assert.Equal(200, transaction.Quantity);
        }

        /// <summary>
        /// Purges all tables used in these tests.
        /// </summary>
        public void Dispose()
        {
            DatabasePurger.PurgeTransactions();
            DatabasePurger.PurgeBatches();
            DatabasePurger.PurgeLocations();
            DatabasePurger.PurgeMaterials();
            DatabasePurger.PurgePlastics();
            DatabasePurger.PurgeUsers();
        }
    }
}
