using OpenMTS.Models;
using System;
using System.Collections.Generic;

namespace OpenMTS.Repositories
{
    /// <summary>
    /// A generic interface for a repository of material batches.
    /// </summary>
    public interface IMaterialBatchRepository
    {
        /// <summary>
        /// Gets all non-archived material batches.  
        /// </summary>
        /// <returns>Returns all material batches</returns>
        IEnumerable<MaterialBatch> GetAllMaterialBatches();

        /// <summary>
        /// Gets and filters material batches. Archived batches are not returned.
        /// </summary>
        /// <param name="materialId">The ID of a material to otionally fitler with.</param>
        /// <param name="siteId">The ID of a storage site to optionally filter with.</param>
        /// <returns>Returns all matching batches.</returns>
        IEnumerable<MaterialBatch> GetMaterialBatches(int? materialId = null, Guid? siteId = null);

        /// <summary>
        /// Gets a material batch by its unique ID.
        /// </summary>
        /// <param name="id">The ID of the batch to get.</param>
        /// <returns>Returns the batch, or null.</returns>
        MaterialBatch GetMaterialBatch(Guid id);

        /// <summary>
        /// Creates a new material batch.
        /// </summary>
        /// <param name="material">The material the batch is composed of..</param>
        /// <param name="expirationDate">The expiration date of the material.</param>
        /// <param name="storageLocation">The storage location of the material.</param>
        /// <param name="batchNumber">The manufacturer provided batch number.</param>
        /// <param name="quantity">The quantity of the batch.</param>
        /// <param name="customProps">The custom prop values fot this batch.</param>
        /// <param name="userId">The ID of the user checking in the new batch..</param>
        /// <returns>Returns the newly created batch.</returns>
        MaterialBatch CreateMaterialBatch(Material material,
            DateTime expirationDate,
            StorageLocation storageLocation,
            long batchNumber,
            double quantity,
            Dictionary<Guid, string> customProps);

        /// <summary>
        /// Updates an existing material batch.
        /// </summary>
        /// <param name="batch">The batch to update.</param>
        void UpdateMaterialBatch(MaterialBatch batch);
    }
}
