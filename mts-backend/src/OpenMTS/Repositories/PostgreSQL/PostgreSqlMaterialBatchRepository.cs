using Dapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using OpenMTS.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace OpenMTS.Repositories.PostgreSQL
{
    /// <summary>
    /// PostgreSQL implementation of the material batch repository.
    /// </summary>
    /// <seealso cref="OpenMTS.Repositories.PostgreSQL.PostgreSqlRepositoryBase" />
    /// <seealso cref="OpenMTS.Repositories.IMaterialBatchRepository" />
    public class PostgreSqlMaterialBatchRepository : PostgreSqlRepositoryBase, IMaterialBatchRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PostgreSqlMaterialBatchRepository"/> class.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        public PostgreSqlMaterialBatchRepository(IConfiguration configuration, ILogger<PostgreSqlMaterialBatchRepository> logger) : base(configuration, logger) { }

        /// <summary>
        /// Gets all non-archived material batches.
        /// </summary>
        /// <returns>
        /// Returns all material batches
        /// </returns>
        /// <exception cref="NotImplementedException"></exception>
        public IEnumerable<MaterialBatch> GetAllMaterialBatches()
        {
            return GetBatches();
        }

        /// <summary>
        /// Gets and filters material batches. Archived batches are not returned.
        /// </summary>
        /// <param name="materialId">The ID of a material to otionally fitler with.</param>
        /// <param name="siteId">The ID of a storage site to optionally filter with.</param>
        /// <returns>
        /// Returns all matching batches.
        /// </returns>
        /// <exception cref="NotImplementedException"></exception>
        public IEnumerable<MaterialBatch> GetMaterialBatches(int? materialId = null, Guid? siteId = null)
        {
            string whereClause = "";
            if (materialId != null)
            {
                whereClause += " m.id=@MaterialId";
            }
            if (siteId != null)
            {
                if (whereClause != "")
                {
                    whereClause += " AND";
                }
                whereClause += " s.id=@SiteId";
            }
            return GetBatches(whereClause, new { materialId, siteId });
        }

        /// <summary>
        /// Gets a material batch by its unique ID.
        /// </summary>
        /// <param name="id">The ID of the batch to get.</param>
        /// <returns>
        /// Returns the batch, or null.
        /// </returns>
        /// <exception cref="NotImplementedException"></exception>
        public MaterialBatch GetMaterialBatch(Guid id)
        {
            IEnumerable<MaterialBatch> batches = GetBatches(" b.id=@Id", new { id }, false);
            return batches.FirstOrDefault();
        }

        /// <summary>
        /// Creates a new material batch.
        /// </summary>
        /// <param name="material">The material the batch is composed of..</param>
        /// <param name="expirationDate">The expiration date of the material.</param>
        /// <param name="storageLocation">The storage location of the material.</param>
        /// <param name="batchNumber">The manufacturer provided batch number.</param>
        /// <param name="quantity">The quantity of the batch.</param>
        /// <param name="customProps">The custom prop values fot this batch.</param>
        /// <param name="isLocked">Whether the batch is locked.</param>
        /// <returns>
        /// Returns the newly created batch.
        /// </returns>
        /// <exception cref="NotImplementedException"></exception>
        public MaterialBatch CreateMaterialBatch(Material material, DateTime expirationDate, StorageLocation storageLocation, long batchNumber, double quantity, Dictionary<Guid, string> customProps, bool isLocked)
        {
            string sql = "INSERT INTO batches (id, material_id, area_id, batch_number, expiration_date, quantity, is_locked, is_archived)" +
                "VALUES (@Id, @MaterialId, @AreaId, @BatchNumber, @ExpirationDate, @Quantity, @IsLocked, @IsArchived)";
            Guid id = Guid.NewGuid();

            IEnumerable<MaterialBatch> batches = null;
            using (IDbConnection connection = GetNewConnection())
            {
                connection.Execute(sql, new
                {
                    id,
                    MaterialId = material.Id,
                    AreaId = storageLocation.StorageAreaId,
                    batchNumber,
                    expirationDate,
                    quantity,
                    isLocked,
                    IsArchived = false
                });
                connection.Execute("INSERT INTO batch_prop_values (batch_id,prop_id,value) VALUES (@BatchId, @PropId, @Value)",
                    customProps.Select(p => new
                    {
                        BatchId = id,
                        PropId = p.Key,
                        p.Value
                    }));
                batches = GetBatches(" b.id=@Id", new { id });
            }
            return batches.First();
        }

        /// <summary>
        /// Updates an existing material batch.
        /// </summary>
        /// <param name="batch">The batch to update.</param>
        /// <exception cref="NotImplementedException"></exception>
        public void UpdateMaterialBatch(MaterialBatch batch)
        {
            string sql = "UPDATE batches SET material_id=@MaterialId, area_id=@AreaId, batch_number=@BatchNumber, " +
                "expiration_date=@ExpirationDate, quantity=@Quantity, is_locked=@IsLocked, is_archived=@IsArchived " +
                   "WHERE id=@Id";

            using (IDbConnection connection = GetNewConnection())
            {
                connection.Execute(sql, new
                {
                    batch.Id,
                    MaterialId = batch.Material.Id,
                    AreaId = batch.StorageLocation.StorageAreaId,
                    batch.BatchNumber,
                    batch.ExpirationDate,
                    batch.Quantity,
                    batch.IsLocked,
                    batch.IsArchived
                });
                connection.Execute("INSERT INTO batch_prop_values (batch_id,prop_id,value) VALUES (@BatchId, @PropId, @Value) ON CONFLICT (batch_id,prop_id) DO UPDATE SET value=@Value",
                    batch.CustomProps.Select(p => new
                    {
                        BatchId = batch.Id,
                        PropId = p.Key,
                        p.Value
                    }));
            }
        }

        #region Private helpers

        /// <summary>
        /// A generic getter of batches. Gets all non-archived batches if called with (null,null).
        /// </summary>
        /// <param name="whereClause">An optional where clause.</param>
        /// <param name="dataParam">The data to fill the where clause with.</param>
        /// <returns>Returns all matching batches</returns>
        public IEnumerable<MaterialBatch> GetBatches(string whereClause = null, object dataParam = null, bool hideArchived = true)
        {
            string sql = "SELECT b.id,b.batch_number,b.expiration_date,b.quantity,b.is_locked,b.is_archived," +
                "a.id AS storage_area_id, a.name AS storage_area_name,s.id AS storage_site_id,s.name AS storage_site_name, " +
                "m.id,m.name,m.manufacturer,m.manufacturer_specific_id, " +
                "p.*, " +
                "v.prop_id,v.value " +
                "FROM batches b " +
                "JOIN storage_areas a ON a.id=b.area_id " +
                "JOIN storage_sites s ON s.id=a.site_id " +
                "JOIN materials m ON m.id=b.material_id " +
                "JOIN plastics p ON p.id=m.type " +
                "LEFT JOIN batch_prop_values v ON v.batch_id=b.id";
            if (hideArchived)
            {
                sql += " WHERE b.is_archived=false";
            }
            if (!string.IsNullOrWhiteSpace(whereClause))
            {
                if (hideArchived)
                {
                    sql += " AND ";
                }
                else
                {
                    sql += " WHERE ";
                }
                sql += whereClause;
            }
            Dictionary<Guid, MaterialBatch> batchMap = new Dictionary<Guid, MaterialBatch>();

            IEnumerable<MaterialBatch> batches = null;
            using (IDbConnection connection = GetNewConnection())
            {
                batches = connection.Query<MaterialBatch, StorageLocation, Material, Plastic, (Guid, string), MaterialBatch>(sql,
                    (bat, location, material, plastic, prop) =>
                    {
                        MaterialBatch batch = null;
                        if (!batchMap.TryGetValue(bat.Id, out batch))
                        {
                            batch = bat;
                            batchMap.Add(batch.Id, batch);
                        }
                        batch.StorageLocation = location;
                        material.Type = plastic;
                        batch.Material = material;
                        if (prop != (default, default))
                        {
                            batch.CustomProps.Add(prop.Item1, prop.Item2);
                        }
                        return batch;
                    },
                    splitOn: "storage_area_id,id,id,prop_id",
                    param: dataParam);
            }
            return batches.Distinct();
        }

        #endregion
    }
}
