using Microsoft.Extensions.Logging.Abstractions;
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
    /// Tests the PostgreSQL custom batch prop repository.
    /// </summary>
    /// <seealso cref="System.IDisposable" />
    public class CustomBatchPropRepositoryTests : IDisposable
    {
        /// <summary>
        /// The SUT: a custom bathc prop repository.
        /// </summary>
        private ICustomBatchPropRepository Repository { get; }

        /// <summary>
        /// Sets up all needed repositories and purges related tables.
        /// </summary>
        public CustomBatchPropRepositoryTests()
        {
            Repository = new PostgreSqlCustomBatchPropRepository(ConfigurationProvider.GetConfiguration(), new NullLogger<PostgreSqlCustomBatchPropRepository>());
            DatabasePurger.PurgeCustomBatchProps();
        }

        /// <summary>
        /// Tests all methods of the custom batch property repository.
        /// </summary>
        [Fact]
        public void TestCustomBatchPropRepository()
        {
            Assert.Empty(Repository.GetAllCustomBatchProps());

            // Create
            Repository.CreateCustomBatchProp("test_prop");
            Repository.CreateCustomBatchProp("Super Prop");

            // Check
            IEnumerable<CustomBatchProp> props = Repository.GetAllCustomBatchProps();
            Assert.Equal(2, props.Count());
            Assert.Single(props.Where(p => p.Name == "test_prop"));
            Assert.Single(props.Where(p => p.Name == "Super Prop"));

            // Update
            CustomBatchProp prop = props.First(p => p.Name == "test_prop");
            prop.Name = "Aloha Prop";
            Repository.UpdateCustomBatchProp(prop);
            prop = Repository.GetCustomBatchProp(prop.Id);
            Assert.Equal("Aloha Prop", prop.Name);

            // Delete
            Repository.DeleteCustomBatchProp(prop.Id);
            props = Repository.GetAllCustomBatchProps();
            Assert.Single(props);
            Assert.Equal("Super Prop", props.Single().Name);
        }

        /// <summary>
        /// Purges custom batch props.
        /// </summary>
        public void Dispose()
        {
            DatabasePurger.PurgeCustomBatchProps();
        }
    }
}
