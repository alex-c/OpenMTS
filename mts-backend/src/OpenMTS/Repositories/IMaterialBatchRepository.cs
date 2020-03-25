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
        /// Gets all material batches.
        /// </summary>
        /// <returns>Returns all material batches</returns>
        IEnumerable<MaterialBatch> GetAllMaterialBatches();

        /// <summary>
        /// Gets and filters material batches.
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
        
        // TODO: params and comment
        MaterialBatch CreateMaterialBatch();

        // TODO: params and comment
        void UpdateMaterialBatch();
    }
}
