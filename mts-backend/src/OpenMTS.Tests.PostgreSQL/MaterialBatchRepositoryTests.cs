using Microsoft.Extensions.Configuration;
using OpenMTS.Models;
using OpenMTS.Repositories;
using OpenMTS.Repositories.PostgreSQL;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace OpenMTS.Tests.PostgreSQL
{
    /// <summary>
    /// Tests the PostgreSQL implementation of the material batch repository.
    /// </summary>
    /// <seealso cref="System.IDisposable" />
    public class MaterialBatchRepositoryTests : IDisposable
    {
        /// <summary>
        /// The SUT: a material batch repository.
        /// </summary>
        private IMaterialBatchRepository Repository { get; }

        // Repos used to create prerequisite entities
        private ILocationsRepository LocationsRepository { get; }
        private IMaterialsRepository MaterialsRepository { get; }
        private IPlasticsRepository PlasticsRepository { get; }
        private ICustomBatchPropRepository PropRepository { get; }

        /// <summary>
        /// Sets up all needed repositories and purges all related tables.
        /// </summary>
        public MaterialBatchRepositoryTests()
        {
            IConfiguration configuration = ConfigurationProvider.GetConfiguration();
            Repository = new PostgreSqlMaterialBatchRepository(configuration);
            LocationsRepository = new PostgreSqlLocationsRepository(configuration);
            MaterialsRepository = new PostgreSqlMaterialsRepository(configuration);
            PlasticsRepository = new PostgreSqlPlasticsRepository(configuration);
            PropRepository = new PostgreSqlCustomBatchPropRepository(configuration);
            Dispose();
        }

        /// <summary>
        /// Tests all methods of the material batch repository.
        /// </summary>
        [Fact]
        public void TestMaterialBatchRepository()
        {
            Assert.Empty(Repository.GetAllMaterialBatches());

            // Create prerequisite entities
            Plastic pp = PlasticsRepository.CreatePlastic("PP", "Polypropylene");
            Plastic ep = PlasticsRepository.CreatePlastic("EP", "Epoxy");
            Material pp1 = MaterialsRepository.CreateMaterial("pp-1", "Umbrella Corp.", "pp-1", pp);
            Material ep1 = MaterialsRepository.CreateMaterial("ep-1", "Umbrella Corp.", "ep-1", ep);
            StorageSite site = LocationsRepository.CreateStorageSite("Test Site");
            StorageArea area = LocationsRepository.CreateStorageArea(site, "Test Area");
            StorageLocation loc = new StorageLocation()
            {
                StorageSiteId = site.Id,
                StorageAreaId = area.Id,
                StorageSiteName = site.Name,
                StorageAreaName = area.Name
            };

            // Create batch and check return
            DateTime expDate = DateTime.Today.AddDays(5);
            MaterialBatch batch = Repository.CreateMaterialBatch(pp1, expDate, loc, 1, 125.0, null, false);
            Assert.Equal(pp1.Id, batch.Material.Id);
            Assert.Equal(expDate, batch.ExpirationDate);
            Assert.Equal(area.Id, batch.StorageLocation.StorageAreaId);
            Assert.Equal(site.Id, batch.StorageLocation.StorageSiteId);
            Assert.Equal(1, batch.BatchNumber);
            Assert.Equal(125.0, batch.Quantity);
            Assert.Empty(batch.CustomProps);
            Assert.False(batch.IsLocked);
            Assert.False(batch.IsArchived);

            // Check get all batches
            IEnumerable<MaterialBatch> batches = Repository.GetAllMaterialBatches();
            Assert.Single(batches);
            batch = batches.Single();
            Assert.Equal(pp1.Id, batch.Material.Id);
            Assert.Equal(expDate, batch.ExpirationDate);
            Assert.Equal(area.Id, batch.StorageLocation.StorageAreaId);
            Assert.Equal(site.Id, batch.StorageLocation.StorageSiteId);
            Assert.Equal(1, batch.BatchNumber);
            Assert.Equal(125.0, batch.Quantity);
            Assert.Empty(batch.CustomProps);
            Assert.False(batch.IsLocked);
            Assert.False(batch.IsArchived);

            // Check get single batch
            batch = Repository.GetMaterialBatch(batch.Id);
            Assert.Equal(pp1.Id, batch.Material.Id);
            Assert.Equal(expDate, batch.ExpirationDate);
            Assert.Equal(area.Id, batch.StorageLocation.StorageAreaId);
            Assert.Equal(site.Id, batch.StorageLocation.StorageSiteId);
            Assert.Equal(1, batch.BatchNumber);
            Assert.Equal(125.0, batch.Quantity);
            Assert.Empty(batch.CustomProps);
            Assert.False(batch.IsLocked);
            Assert.False(batch.IsArchived);

            // Check filters
            batches = Repository.GetMaterialBatches(pp1.Id);
            Assert.Single(batches);
            batch = batches.Single();
            Assert.Equal(pp1.Id, batch.Material.Id);
            batches = Repository.GetMaterialBatches(siteId: site.Id);
            Assert.Single(batches);
            batch = batches.Single();
            Assert.Equal(pp1.Id, batch.Material.Id);
            batches = Repository.GetMaterialBatches(pp1.Id, site.Id);
            Assert.Single(batches);
            batch = batches.Single();
            Assert.Equal(pp1.Id, batch.Material.Id);
            batches = Repository.GetMaterialBatches(99999, site.Id);
            Assert.Empty(batches);
            batches = Repository.GetMaterialBatches(pp1.Id, area.Id);
            Assert.Empty(batches);

            // Create another batch and test filters again
            Repository.CreateMaterialBatch(ep1, expDate, loc, 33, 25.5, new Dictionary<Guid, string>(), false);
            batches = Repository.GetMaterialBatches(ep1.Id, site.Id);
            Assert.Single(batches);
            batch = batches.Single();
            Assert.Equal(ep1.Id, batch.Material.Id);
            batches = Repository.GetMaterialBatches(siteId: site.Id);
            Assert.Equal(2, batches.Count());
            Assert.Single(batches, b => b.Material.Id == pp1.Id);
            Assert.Single(batches, b => b.Material.Id == ep1.Id);

            // Test updating a batch
            batch = batches.Single(b => b.Material.Id == ep1.Id);
            Assert.Equal(33, batch.BatchNumber);
            Assert.Equal(25.5, batch.Quantity);
            Assert.False(batch.IsLocked);
            batch.BatchNumber = 333;
            batch.Quantity = 203.1;
            batch.IsLocked = true;
            Repository.UpdateMaterialBatch(batch);
            batch = Repository.GetMaterialBatch(batch.Id);
            Assert.Equal(333, batch.BatchNumber);
            Assert.Equal(203.1, batch.Quantity);
            Assert.True(batch.IsLocked);

            // Test archiving
            Guid archivedId = batch.Id;
            batch.IsArchived = true;
            Repository.UpdateMaterialBatch(batch);
            batches = Repository.GetAllMaterialBatches();
            Assert.Single(batches);
            batch = batches.Single();
            Assert.Equal(pp1.Id, batch.Material.Id);

            // Still directly gettable by ID
            batch = Repository.GetMaterialBatch(archivedId);
            Assert.Equal(ep1.Id, batch.Material.Id);
        }

        /// <summary>
        /// Tests the setting and getting of custom batch prop values.
        /// </summary>
        [Fact]
        public void TestCustomBatchPropValues()
        {
            // Prerequisite entities
            Plastic pp = PlasticsRepository.CreatePlastic("PP", "Polypropylene");
            Material mat = MaterialsRepository.CreateMaterial("mat", "manu", "manu-id", pp);
            StorageSite site = LocationsRepository.CreateStorageSite("test_site");
            StorageArea area = LocationsRepository.CreateStorageArea(site, "test_area");
            StorageLocation loc = new StorageLocation()
            {
                StorageSiteId = site.Id,
                StorageAreaId = area.Id,
                StorageSiteName = site.Name,
                StorageAreaName = area.Name
            };
            CustomBatchProp prop1 = PropRepository.CreateCustomBatchProp("prop-1");
            CustomBatchProp prop2 = PropRepository.CreateCustomBatchProp("prop-2");

            // Create batch with custom prop values
            MaterialBatch batch = Repository.CreateMaterialBatch(mat, DateTime.Today.AddDays(17), loc, 42, 42, new Dictionary<Guid, string>()
            {
                {prop1.Id, "Ak Bars"},
                {prop2.Id, "Aloha"}
            }, false);
            Assert.Equal(2, batch.CustomProps.Count);

            // Test updating
            batch.CustomProps[prop1.Id] = "UPDATE TEST";
            Repository.UpdateMaterialBatch(batch);
            batch = Repository.GetMaterialBatch(batch.Id);
            Assert.Equal(2, batch.CustomProps.Count);
            Assert.Single(batch.CustomProps.Where(p => p.Value == "UPDATE TEST"));
        }

        /// <summary>
        /// Purges all tables used in these tests.
        /// </summary>
        public void Dispose()
        {
            DatabasePurger.PurgeBatches();
            DatabasePurger.PurgeLocations();
            DatabasePurger.PurgeMaterials();
            DatabasePurger.PurgePlastics();
        }
    }
}
