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
    /// Tests the PostgreSQL implementation of the materials repository. This assumes that the plastics repository
    /// is already succesfully tested.
    /// </summary>
    /// <seealso cref="System.IDisposable" />
    public class MaterialsRepositoryTests : IDisposable
    {
        /// <summary>
        /// The SUT: a materials repository.
        /// </summary>
        private IMaterialsRepository Repository { get; }

        // Plastics used in testing
        private Plastic PC { get; }
        private Plastic PE { get; }

        /// <summary>
        /// Sets up needed repositories and purges all related tables.
        /// </summary>
        public MaterialsRepositoryTests()
        {
            // Repos
            Repository = new PostgreSqlMaterialsRepository(ConfigurationProvider.GetConfiguration());
            IPlasticsRepository plasticsRepository = new PostgreSqlPlasticsRepository(ConfigurationProvider.GetConfiguration());

            // Purge for blank slate
            DatabasePurger.PurgeMaterials();
            DatabasePurger.PurgePlastics();

            // Create plastics
            PC = plasticsRepository.CreatePlastic("PC", "Polycarbonate");
            PE = plasticsRepository.CreatePlastic("PE", "Polyethylene");
        }

        /// <summary>
        /// Tests all methods of the materials repository.
        /// </summary>
        [Fact]
        public void TestMaterialsRepository()
        {
            Assert.Empty(Repository.GetAllMaterials());

            // Create materials
            Material m1 = Repository.CreateMaterial("m1", "Alpha", "alpha-m1", PC);
            Material m2 = Repository.CreateMaterial("m2", "Alpha", "alpha-m2", PE);
            Material m3 = Repository.CreateMaterial("m3", "Beta", "beta-m1", PC);
            Assert.Equal("m1", m1.Name);
            Assert.Equal("m2", m2.Name);
            Assert.Equal("m3", m3.Name);

            // Check materials
            IEnumerable<Material> materials = Repository.GetAllMaterials();
            Assert.Equal(3, materials.Count());
            Assert.Contains(materials, m => m.Name == "m1" && m.Manufacturer == "Alpha" && m.Type.Id == PC.Id && m.Type.Name == PC.Name);
            Assert.Contains(materials, m => m.Name == "m2" && m.Manufacturer == "Alpha" && m.Type.Id == PE.Id && m.Type.Name == PE.Name);
            Assert.Contains(materials, m => m.Name == "m3" && m.Manufacturer == "Beta" && m.Type.Id == PC.Id && m.Type.Name == PC.Name);

            // Test filters - name
            materials = Repository.GetFilteredMaterials("m1");
            Assert.Single(materials);
            Assert.Contains(materials, m => m.Name == "m1" && m.Manufacturer == "Alpha" && m.Type.Id == PC.Id && m.Type.Name == PC.Name);

            // Test filters - manufacturer
            materials = Repository.GetFilteredMaterials(partialManufacturerName: "alp");
            Assert.Equal(2, materials.Count());
            Assert.Contains(materials, m => m.Name == "m1" && m.Manufacturer == "Alpha" && m.Type.Id == PC.Id && m.Type.Name == PC.Name);
            Assert.Contains(materials, m => m.Name == "m2" && m.Manufacturer == "Alpha" && m.Type.Id == PE.Id && m.Type.Name == PE.Name);

            // Test filters - type
            materials = Repository.GetFilteredMaterials(type: PC);
            Assert.Equal(2, materials.Count());
            Assert.Contains(materials, m => m.Name == "m1" && m.Manufacturer == "Alpha" && m.Type.Id == PC.Id && m.Type.Name == PC.Name);
            Assert.Contains(materials, m => m.Name == "m3" && m.Manufacturer == "Beta" && m.Type.Id == PC.Id && m.Type.Name == PC.Name);

            // Test - multi filters
            materials = Repository.GetFilteredMaterials(partialManufacturerName: "lph", type: PC);
            Assert.Single(materials);
            Assert.Contains(materials, m => m.Name == "m1" && m.Manufacturer == "Alpha" && m.Type.Id == PC.Id && m.Type.Name == PC.Name);

            // Get single material
            int id = materials.First().Id;
            Material material = Repository.GetMaterial(id);
            Assert.NotNull(material);
            Assert.True(material.Name == "m1" && material.Manufacturer == "Alpha" && material.Type.Id == PC.Id && material.Type.Name == PC.Name);

            // Update material
            material.Name = "m9000";
            material.Manufacturer = "Umbrella Corp.";
            material.ManufacturerSpecificId = "secret";
            material.Type = PE;
            Repository.UpdateMaterial(material);
            material = Repository.GetMaterial(id);
            Assert.NotNull(material);
            Assert.True(material.Name == "m9000" && material.Manufacturer == "Umbrella Corp." && material.ManufacturerSpecificId == "secret" && material.Type.Id == PE.Id && material.Type.Name == PE.Name);
        }

        /// <summary>
        /// Purges materials and plastics.
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        public void Dispose()
        {
            DatabasePurger.PurgeMaterials();
            DatabasePurger.PurgePlastics();
        }
    }
}
