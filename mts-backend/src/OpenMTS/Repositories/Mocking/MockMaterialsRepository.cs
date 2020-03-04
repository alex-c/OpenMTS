using OpenMTS.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OpenMTS.Repositories.Mocking
{
    public class MockMaterialsRepository : IMaterialsRepository
    {
        private Dictionary<int, Material> Materials { get; }

        public MockMaterialsRepository(MockDataProvider mockDataProvider)
        {
            if (mockDataProvider == null)
            {
                Materials = new Dictionary<int, Material>();
            }
            else
            {
                Materials = mockDataProvider.Materials;
            }
        }

        public IEnumerable<Material> GetAllMaterials()
        {
            return Materials.Values;
        }

        public IEnumerable<Material> GetFilteredMaterials(string partialName, string partialManufacturerName, MaterialType? type)
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
                materials = materials.Where(m => m.Type == type);
            }
            return materials;
        }
    }
}
