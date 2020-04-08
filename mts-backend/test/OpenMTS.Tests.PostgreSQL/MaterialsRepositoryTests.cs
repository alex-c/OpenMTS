using Microsoft.Extensions.Configuration;
using OpenMTS.Models;
using OpenMTS.Repositories;
using OpenMTS.Repositories.PostgreSQL;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xunit;

namespace OpenMTS.Tests.PostgreSQL
{
    /// <summary>
    /// Tests the PostgreSQL implementation of the materials repository and the custom material prop values
    /// repository. This assumes that the plastics and custom prop repositories are already succesfully tested.
    /// </summary>
    /// <seealso cref="System.IDisposable" />
    public class MaterialsRepositoryTests : IDisposable
    {
        /// <summary>
        /// The primary SUT: a materials repository.
        /// </summary>
        private IMaterialsRepository Repository { get; }

        /// <summary>
        /// The secondary SUT: a materials repository.
        /// </summary>
        private ICustomMaterialPropValueRepository PropValueRepository { get; }

        /// <summary>
        /// Used to create props to test the value repository with.
        /// </summary>
        private ICustomMaterialPropRepository PropRepository { get; }

        // Plastics used in testing
        private Plastic PC { get; }
        private Plastic PE { get; }

        /// <summary>
        /// Sets up needed repositories and purges all related tables.
        /// </summary>
        public MaterialsRepositoryTests()
        {
            // Repos
            IConfiguration configuration = ConfigurationProvider.GetConfiguration();
            Repository = new PostgreSqlMaterialsRepository(configuration);
            PropRepository = new PostgreSqlCustomMaterialPropRepository(configuration);
            PropValueRepository = new PostgreSqlCustomMaterialPropValueRepository(configuration);
            IPlasticsRepository plasticsRepository = new PostgreSqlPlasticsRepository(configuration);

            // Purge for blank slate
            DatabasePurger.PurgeMaterials();
            DatabasePurger.PurgePlastics();
            DatabasePurger.PurgeCustomMaterialProps();

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
        /// Tests the setting and removing of custom material property values.
        /// </summary>
        [Fact]
        public void TestCustomMaterialPropValues()
        {
            // Create materials
            Material m1 = Repository.CreateMaterial("m1", "Alpha", "alpha-m1", PC);
            Material m2 = Repository.CreateMaterial("m2", "Alpha", "alpha-m2", PE);
            Material m3 = Repository.CreateMaterial("m3", "Beta", "beta-m1", PC);

            // Create custom props
            CustomMaterialProp textProp = PropRepository.CreateCustomMaterialProp("test", PropType.Text);
            CustomMaterialProp fileProp = PropRepository.CreateCustomMaterialProp("file", PropType.Text);

            // Test getters with no prop values
            IEnumerable<Material> materials = Repository.GetAllMaterials();
            Assert.Equal(3, materials.Count());
            Assert.Empty(materials.Where(m => m.CustomProps.Count > 0));

            // Set custom prop values
            PropValueRepository.SetCustomTextMaterialProp(m1.Id, textProp.Id, "Lorem Ipsum");
            PropValueRepository.SetCustomFileMaterialProp(m1.Id, fileProp.Id, "Z:/my-files/file.pdf");
            PropValueRepository.SetCustomFileMaterialProp(m2.Id, fileProp.Id, "X:/formatc.cmd");

            // Test custom prop values
            materials = Repository.GetAllMaterials();
            m1 = materials.Single(m => m.Name == "m1");
            m2 = materials.Single(m => m.Name == "m2");
            m3 = materials.Single(m => m.Name == "m3");
            Assert.Equal(2, m1.CustomProps.Count);
            Assert.Single(m2.CustomProps);
            Assert.Empty(m3.CustomProps);
            Assert.Single(m1.CustomProps.Where(p => p.PropId == textProp.Id && (string)p.Value == "Lorem Ipsum"));
            Assert.Single(m1.CustomProps.Where(p => p.PropId == fileProp.Id && (string)p.Value == "Z:/my-files/file.pdf"));
            Assert.Single(m2.CustomProps.Where(p => p.PropId == fileProp.Id && (string)p.Value == "X:/formatc.cmd"));

            // Overwrite custom prop value
            PropValueRepository.SetCustomTextMaterialProp(m1.Id, textProp.Id, "Foo Bar");
            m1 = Repository.GetMaterial(m1.Id);
            Assert.Equal(2, m1.CustomProps.Count);
            Assert.Empty(m1.CustomProps.Where(p => p.PropId == textProp.Id && (string)p.Value == "Lorem Ipsum"));
            Assert.Single(m1.CustomProps.Where(p => p.PropId == textProp.Id && (string)p.Value == "Foo Bar"));
            Assert.Single(m1.CustomProps.Where(p => p.PropId == fileProp.Id && (string)p.Value == "Z:/my-files/file.pdf"));

            // Add second text prop
            CustomMaterialProp textProp2 = PropRepository.CreateCustomMaterialProp("troll", PropType.Text);
            PropValueRepository.SetCustomTextMaterialProp(m1.Id, textProp2.Id, "Ak Bars");
            m1 = Repository.GetMaterial(m1.Id);
            Assert.Equal(3, m1.CustomProps.Count);
            Assert.Single(m1.CustomProps.Where(p => p.PropId == textProp.Id && (string)p.Value == "Foo Bar"));
            Assert.Single(m1.CustomProps.Where(p => p.PropId == textProp2.Id && (string)p.Value == "Ak Bars"));
            Assert.Single(m1.CustomProps.Where(p => p.PropId == fileProp.Id && (string)p.Value == "Z:/my-files/file.pdf"));

            // Add second file prop
            CustomMaterialProp fileProp2 = PropRepository.CreateCustomMaterialProp("super-file", PropType.Text);
            PropValueRepository.SetCustomTextMaterialProp(m2.Id, fileProp2.Id, "F:/ile/path");
            m2 = Repository.GetMaterial(m2.Id);
            Assert.Equal(2, m2.CustomProps.Count);
            Assert.Single(m2.CustomProps.Where(p => p.PropId == fileProp.Id && (string)p.Value == "X:/formatc.cmd"));
            Assert.Single(m2.CustomProps.Where(p => p.PropId == fileProp2.Id && (string)p.Value == "F:/ile/path"));

            // Check getAll again
            materials = Repository.GetAllMaterials();
            m1 = materials.Single(m => m.Name == "m1");
            m2 = materials.Single(m => m.Name == "m2");
            m3 = materials.Single(m => m.Name == "m3");
            Assert.Equal(3, m1.CustomProps.Count);
            Assert.Equal(2, m2.CustomProps.Count);
            Assert.Empty(m3.CustomProps);

            // Check getter with filters
            materials = Repository.GetFilteredMaterials(null, "alpha", PE);
            Assert.Single(materials);
            m2 = materials.Single();
            Assert.Equal(2, m2.CustomProps.Count);
            Assert.Single(m2.CustomProps.Where(p => p.PropId == fileProp.Id && (string)p.Value == "X:/formatc.cmd"));
            Assert.Single(m2.CustomProps.Where(p => p.PropId == fileProp2.Id && (string)p.Value == "F:/ile/path"));

            // Test removal of prop values
            PropValueRepository.RemoveCustomTextMaterialProp(m1.Id, textProp.Id);
            PropValueRepository.RemoveCustomFileMaterialProp(m2.Id, fileProp.Id);
            m1 = Repository.GetMaterial(m1.Id);
            m2 = Repository.GetMaterial(m2.Id);
            Assert.Equal(2, m1.CustomProps.Count);
            Assert.Single(m1.CustomProps.Where(p => p.PropId == textProp2.Id && (string)p.Value == "Ak Bars"));
            Assert.Single(m1.CustomProps.Where(p => p.PropId == fileProp.Id && (string)p.Value == "Z:/my-files/file.pdf"));
            Assert.Single(m2.CustomProps);
            Assert.Single(m2.CustomProps.Where(p => p.PropId == fileProp2.Id && (string)p.Value == "F:/ile/path"));
        }

        /// <summary>
        /// Purges all tables used in these tests.
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        public void Dispose()
        {
            DatabasePurger.PurgeMaterials();
            DatabasePurger.PurgePlastics();
            DatabasePurger.PurgeCustomMaterialProps();
        }
    }
}
