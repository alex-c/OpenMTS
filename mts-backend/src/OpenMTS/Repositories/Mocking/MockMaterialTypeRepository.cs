using OpenMTS.Models;
using System.Collections.Generic;
using System.Linq;

namespace OpenMTS.Repositories.Mocking
{
    public class MockMaterialTypeRepository : IMaterialTypeRepository
    {
        private Dictionary<string, MaterialType> MaterialTypes { get; }

        public MockMaterialTypeRepository(MockDataProvider mockDataProvider = null)
        {
            if (mockDataProvider == null)
            {
                MaterialTypes = new Dictionary<string, MaterialType>();
            }
            else
            {
                MaterialTypes = mockDataProvider.MaterialTypes;
            }
        }

        public IEnumerable<MaterialType> GetAllMaterialTypes()
        {
            return MaterialTypes.Values;
        }

        public IEnumerable<MaterialType> SearchMaterialTypesByName(string partialName)
        {
            return MaterialTypes.Values.Where(m => m.Name.ToLowerInvariant().Contains(partialName.ToLowerInvariant()));
        }

        public MaterialType GetMaterialType(string id)
        {
            return MaterialTypes.GetValueOrDefault(id);
        }

        public MaterialType CreateMaterialType(string id, string name)
        {
            MaterialType type = new MaterialType() { Id = id, Name = name };
            MaterialTypes.Add(id, type);
            return type;
        }

        public void UpdateMaterialType(MaterialType materialType)
        {
            MaterialTypes[materialType.Id] = materialType;
        }

        public void DeleteMaterialType(string id)
        {
            MaterialTypes.Remove(id);
        }
    }
}
