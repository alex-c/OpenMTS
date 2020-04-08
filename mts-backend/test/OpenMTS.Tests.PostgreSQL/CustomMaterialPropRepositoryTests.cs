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
    /// Tests for the PostgreSQL implementation of the custom material props respository.
    /// </summary>
    /// <seealso cref="System.IDisposable" />
    public class CustomMaterialPropRepositoryTests : IDisposable
    {
        /// <summary>
        /// The SUT: a repository of custom material props.
        /// </summary>
        private ICustomMaterialPropRepository Repository { get; }

        /// <summary>
        /// Sets up needed repositories and purges all related tables.
        /// </summary>
        public CustomMaterialPropRepositoryTests()
        {
            Repository = new PostgreSqlCustomMaterialPropRepository(ConfigurationProvider.GetConfiguration());
            DatabasePurger.PurgeCustomMaterialProps();
        }

        /// <summary>
        /// Tests all custom material property repository.
        /// </summary>
        [Fact]
        public void TestCustomMaterialPropRepository()
        {
            Assert.Empty(Repository.GetAllCustomMaterialProps());

            // Create props
            CustomMaterialProp prop1 = Repository.CreateCustomMaterialProp("Text", PropType.Text);
            CustomMaterialProp prop2 = Repository.CreateCustomMaterialProp("File", PropType.File);
            Assert.Equal(2, Repository.GetAllCustomMaterialProps().Count());
            Assert.Equal("Text", prop1.Name);
            Assert.Equal("File", prop2.Name);
            Assert.Equal(PropType.Text, prop1.Type);
            Assert.Equal(PropType.File, prop2.Type);

            // Get single prop
            prop1 = Repository.GetCustomMaterialProp(prop1.Id);
            Assert.Equal("Text", prop1.Name);
            Assert.Equal(PropType.Text, prop1.Type);

            // Update prop
            prop1.Name = "Datenblatt";
            prop1.Type = PropType.File;
            Repository.UpdateCustomMaterialProp(prop1);
            prop1 = Repository.GetCustomMaterialProp(prop1.Id);
            Assert.Equal("Datenblatt", prop1.Name);
            Assert.Equal(PropType.File, prop1.Type);

            // Delete prop
            Repository.DeleteCustomMaterialProp(prop1.Id);
            IEnumerable<CustomMaterialProp> props = Repository.GetAllCustomMaterialProps();
            Assert.Single(props);
            CustomMaterialProp prop = props.Single();
            Assert.Equal("File", prop.Name);
            Assert.Equal(PropType.File, prop.Type);
        }

        /// <summary>
        /// Purges all custom material props.
        /// </summary>
        public void Dispose()
        {
            DatabasePurger.PurgeCustomMaterialProps();
        }
    }
}
