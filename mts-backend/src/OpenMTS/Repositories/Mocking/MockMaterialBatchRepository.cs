using OpenMTS.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OpenMTS.Repositories.Mocking
{
    public class MockMaterialBatchRepository : IMaterialBatchRepository
    {
        private Dictionary<Guid, MaterialBatch> MaterialBatches { get; }

        public MockMaterialBatchRepository(MockDataProvider mockDataProvider = null)
        {
            if (mockDataProvider == null)
            {
                MaterialBatches = new Dictionary<Guid, MaterialBatch>();
            }
            else
            {
                MaterialBatches = mockDataProvider.MaterialBatches;
            }
        }

        public IEnumerable<MaterialBatch> GetAllMaterialBatches()
        {
            return MaterialBatches.Values;
        }

        public IEnumerable<MaterialBatch> GetMaterialBatches(int? materialId = null, Guid? siteId = null)
        {
            IEnumerable<MaterialBatch> batches = MaterialBatches.Values;
            if (materialId.HasValue)
            {
                batches = batches.Where(b => b.Material.Id == materialId.Value);
            }
            if (siteId.HasValue)
            {
                batches = batches.Where(b => b.StorageLocation.StorageSiteId == siteId.Value);
            }
            return batches;
        }

        public MaterialBatch GetMaterialBatch(Guid id)
        {
            return MaterialBatches.GetValueOrDefault(id);
        }

        public MaterialBatch CreateMaterialBatch()
        {
            throw new NotImplementedException();
        }

        public void UpdateMaterialBatch()
        {
            throw new NotImplementedException();
        }
    }
}
