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
            return MaterialBatches.Values.Where(b => !b.IsArchived);
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
            return batches.Where(b => !b.IsArchived);
        }

        public MaterialBatch GetMaterialBatch(Guid id)
        {
            return MaterialBatches.GetValueOrDefault(id);
        }

        public MaterialBatch CreateMaterialBatch(Material material,
            DateTime expirationDate,
            StorageLocation storageLocation,
            long batchNumber,
            double quantity,
            Dictionary<Guid, string> customProps)
        {
            MaterialBatch batch = new MaterialBatch()
            {
                Id = Guid.NewGuid(),
                Material = material,
                ExpirationDate = expirationDate,
                StorageLocation = storageLocation,
                BatchNumber = batchNumber,
                Quantity = quantity,
                CustomProps = customProps,
                IsLocked = false,
                IsArchived = false
            };
            MaterialBatches.Add(batch.Id, batch);
            return batch;
        }

        public void UpdateMaterialBatch(MaterialBatch batch)
        {
            MaterialBatches[batch.Id] = batch;
        }
    }
}
