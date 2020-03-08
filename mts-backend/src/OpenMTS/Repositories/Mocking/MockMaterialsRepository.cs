using OpenMTS.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OpenMTS.Repositories.Mocking
{
    public class MockMaterialsRepository : IMaterialsRepository
    {
        private Dictionary<int, Material> Materials { get; }

        private int LastId { get; set; }

        public MockMaterialsRepository(MockDataProvider mockDataProvider)
        {
            if (mockDataProvider == null)
            {
                Materials = new Dictionary<int, Material>();
                LastId = 0;
            }
            else
            {
                Materials = mockDataProvider.Materials;
                LastId = Materials.Keys.Max();
            }
        }

        public IEnumerable<Material> GetAllMaterials()
        {
            return Materials.Values;
        }

        public IEnumerable<Material> GetFilteredMaterials(string partialName, string partialManufacturerName, MaterialType type)
        {
            IEnumerable<Material> materials = Materials.Values;
            if (partialName != null)
            {
                materials = materials.Where(m => m.Name.ToLowerInvariant().Contains(partialName.ToLowerInvariant()));
            }
            if (partialManufacturerName != null)
            {
                materials = materials.Where(m => m.Manufacturer.ToLowerInvariant().Contains(partialManufacturerName.ToLowerInvariant()));
            }
            if (type != null)
            {
                materials = materials.Where(m => m.Type.Id == type.Id);
            }
            return materials;
        }

        public Material GetMaterial(int id)
        {
            return Materials.GetValueOrDefault(id);
        }

        public Material CreateMaterial(string name, string manufacturerName, string manufacturerSpecificId, MaterialType materialType)
        {
            Material material = new Material()
            {
                Id = GetNextId(),
                Name = name,
                Manufacturer = manufacturerName,
                ManufacturerSpecificId = manufacturerSpecificId,
                Type = materialType,
                CustomProps = new List<CustomMaterialPropValue>()
            };
            Materials.Add(material.Id, material);
            return material;
        }

        public void UpdateMaterial(Material material)
        {
            Materials[material.Id] = material;
        }

        private int GetNextId()
        {
            return ++LastId;
        }
    }
}
