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
    /// Tests the PostgreSQL implementation of the locations repository.,
    /// </summary>
    /// <seealso cref="System.IDisposable" />
    public class LocationsRepositoryTests : IDisposable
    {
        /// <summary>
        /// The SUT: a locations repository.
        /// </summary>
        private ILocationsRepository Repository { get; }

        /// <summary>
        /// Sets up the SUT and needed services and and cleans up locations tbales.
        /// </summary>
        public LocationsRepositoryTests()
        {
            Repository = new PostgreSqlLocationsRepository(ConfigurationProvider.GetConfiguration());
            DatabasePurger.PurgeLocations();
        }

        /// <summary>
        /// Tests all locations repository methods.
        /// </summary>
        [Fact]
        public void TestLocationsRepository()
        {
            Assert.Empty(Repository.GetAllStorageSites());

            // Create sites
            StorageSite site1 = Repository.CreateStorageSite("Test Site 1");
            StorageSite site2 = Repository.CreateStorageSite("Test Site 2");
            Guid site1Id = site1.Id;
            Guid site2Id = site2.Id;

            // Get and check sites
            IEnumerable<StorageSite> sites = Repository.GetAllStorageSites();
            Assert.Equal(2, sites.Count());
            Assert.Contains(sites, s => s.Id == site1.Id && s.Name == site1.Name);
            Assert.Contains(sites, s => s.Id == site2.Id && s.Name == site2.Name);

            // Search by name
            sites = Repository.SearchStorageSitesByName("1");
            Assert.Single(sites);
            Assert.Equal(site1Id, sites.First().Id);

            // Create areas
            Repository.CreateStorageArea(site1, "Area 1");
            Repository.CreateStorageArea(site1, "Area 2");
            Repository.CreateStorageArea(site2, "Area 3");

            // Get and check sites
            sites = Repository.GetAllStorageSites();
            Assert.Equal(2, sites.Count());
            site1 = sites.Single(s => s.Id == site1Id);
            site2 = sites.Single(s => s.Id == site2Id);
            Assert.Equal(2, site1.Areas.Count);
            Assert.Single(site2.Areas);
            Assert.Contains(site1.Areas, a => a.Name == "Area 1");
            Assert.Contains(site1.Areas, a => a.Name == "Area 2");
            Assert.Contains(site2.Areas, a => a.Name == "Area 3");

            // Get site by ID
            site2 = Repository.GetStorageSite(site2Id);
            Assert.Equal(site2Id, site2.Id);
            Assert.Equal("Test Site 2", site2.Name);

            // Update site name
            site1.Name = "NEW NAME";
            Repository.UpdateStorageSite(site1);
            site1 = Repository.GetStorageSite(site1.Id);
            Assert.Equal("NEW NAME", site1.Name);

            // Update area name
            StorageArea area = site1.Areas.First();
            area.Name = "NEW AREA NAME";
            Repository.UpdateStorageArea(area);
            site1 = Repository.GetStorageSite(site1.Id);
            Assert.Contains(site1.Areas, a => a.Id == area.Id && area.Name == area.Name);

            // Delete site
            Repository.DeleteStorageSite(site1Id);
            Assert.Single(Repository.GetAllStorageSites());
        }

        /// <summary>
        /// Pruges all locations.
        /// </summary>
        public void Dispose()
        {
            DatabasePurger.PurgeLocations();
        }
    }
}
