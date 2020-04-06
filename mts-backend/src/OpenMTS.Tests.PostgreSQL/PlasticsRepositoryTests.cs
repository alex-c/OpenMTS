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
    /// Tests for the PostgreSQL-implementation of the plastics repository.
    /// </summary>
    public class PlasticsRepositoryTests : IDisposable
    {
        /// <summary>
        /// The SUT: a plastics repositoy.
        /// </summary>
        private IPlasticsRepository Repository { get; }

        /// <summary>
        /// Sets up the SUT and needed services and and cleans up API key tbales.
        /// </summary>
        public PlasticsRepositoryTests()
        {
            Repository = new PostgreSqlPlasticsRepository(ConfigurationProvider.GetConfiguration());
            DatabasePurger.PurgePlastics();
        }

        /// <summary>
        /// Tests all methods of the plastics repository.
        /// </summary>
        [Fact]
        public void TestPlasticsRepository()
        {
            Assert.Empty(Repository.GetAllPlastics());

            // Create
            Repository.CreatePlastic("PP", "Polypropylene");
            Repository.CreatePlastic("EP", "Epoxy");

            IEnumerable<Plastic> plastics = Repository.GetAllPlastics();
            Assert.Equal(2, plastics.Count());
            Assert.Contains(plastics, p => p.Id == "PP" && p.Name == "Polypropylene");
            Assert.Contains(plastics, p => p.Id == "EP" && p.Name == "Epoxy");

            // Filter
            plastics = Repository.SearchPlasticsByName("poly");
            Assert.Single(plastics);
            Plastic pp = plastics.First();
            Assert.True(pp.Id == "PP" && pp.Name == "Polypropylene");

            // Update
            pp.Name = "Superpolypropylene of Doom";
            Repository.UpdatePlastic(pp);

            plastics = Repository.GetAllPlastics();
            Assert.Equal(2, plastics.Count());
            Assert.Contains(plastics, p => p.Id == "PP" && p.Name == "Superpolypropylene of Doom");
            Assert.Contains(plastics, p => p.Id == "EP" && p.Name == "Epoxy");

            // Delete
            Repository.DeletePlastic("EP");

            plastics = Repository.GetAllPlastics();
            Assert.Single(plastics);
            pp = plastics.First();
            Assert.True(pp.Id == "PP" && pp.Name == "Superpolypropylene of Doom");
        }

        /// <summary>
        /// Purges
        /// </summary>
        public void Dispose()
        {
            DatabasePurger.PurgePlastics();
        }
    }
}
