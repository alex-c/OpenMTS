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
    /// A PostgreSQL implementation of the materials repository.
    /// </summary>
    /// <seealso cref="OpenMTS.Repositories.PostgreSQL.PostgreSqlRepositoryBase" />
    /// <seealso cref="OpenMTS.Repositories.IMaterialsRepository" />
    public class PostgreSqlMaterialsRepository : PostgreSqlRepositoryBase, IMaterialsRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PostgreSqlMaterialsRepository"/> class.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        public PostgreSqlMaterialsRepository(IConfiguration configuration, ILogger<PostgreSqlMaterialsRepository> logger) : base(configuration, logger) { }

        /// <summary>
        /// Gets all materials.
        /// </summary>
        /// <returns>
        /// Returns all materials
        /// </returns>
        public IEnumerable<Material> GetAllMaterials()
        {
            return GetMaterials(null, null);
        }

        /// <summary>
        /// Gets and filters materials.
        /// </summary>
        /// <param name="partialName">A string to filter material names with.</param>
        /// <param name="partialManufacturerName">A string to filter manufacturers with.</param>
        /// <param name="type">The type to filter with.</param>
        /// <returns>
        /// Returns the matching materials.
        /// </returns>
        public IEnumerable<Material> GetFilteredMaterials(string partialName = null, string partialManufacturerName = null, Plastic type = null)
        {
            // Build where clause
            string whereClause = "";
            if (partialName != null)
            {
                partialName = $"%{partialName}%";
                whereClause += "m.name ILIKE @Name ";
            }
            if (partialManufacturerName != null)
            {
                if (whereClause != "")
                {
                    whereClause += "AND ";
                }
                partialManufacturerName = $"%{partialManufacturerName}%";
                whereClause += "m.manufacturer ILIKE @Manufacturer ";
            }
            if (type != null)
            {
                if (whereClause != "")
                {
                    whereClause += "AND ";
                }
                whereClause += $"m.type=@Type ";
            }
            if (whereClause != "")
            {
                whereClause = $" WHERE {whereClause}";
            }

            // Query
            return GetMaterials(whereClause, new
            {
                Name = partialName,
                Manufacturer = partialManufacturerName,
                Type = type?.Id
            });
        }

        /// <summary>
        /// Gets a material by it's ID.
        /// </summary>
        /// <param name="id">The ID of the material to get.</param>
        /// <returns>
        /// Returns the material or null.
        /// </returns>
        public Material GetMaterial(int id)
        {
            IEnumerable<Material> materials = GetMaterials("WHERE m.id=@Id", new { id });
            return materials.FirstOrDefault();
        }

        /// <summary>
        /// Creates a new material.
        /// </summary>
        /// <param name="name">The new material's name.</param>
        /// <param name="manufacturerName">Name of the manufacturer.</param>
        /// <param name="manufacturerSpecificId">The manufacturer's ID for this material.</param>
        /// <param name="type"></param>
        /// <returns>
        /// Retursn the newly created material.
        /// </returns>
        public Material CreateMaterial(string name, string manufacturerName, string manufacturerSpecificId, Plastic type)
        {
            IEnumerable<Material> materials = null;
            using (IDbConnection connection = GetNewConnection())
            {
                int id = connection.QuerySingle<int>("INSERT INTO materials (name, manufacturer, manufacturer_specific_id, type) VALUES (@Name, @Manufacturer, @ManufacturerSpecificId, @Type) RETURNING id",
                    param: new
                    {
                        Name = name,
                        Manufacturer = manufacturerName,
                        ManufacturerSpecificId = manufacturerSpecificId,
                        Type = type.Id
                    });
                materials = GetMaterials("WHERE m.id=@Id", new { id });
            }
            return materials.First();
        }

        /// <summary>
        /// Updates an existing material.
        /// </summary>
        /// <param name="material">The material to update.</param>
        public void UpdateMaterial(Material material)
        {
            using (IDbConnection connection = GetNewConnection())
            {
                connection.Execute("UPDATE materials SET name=@Name, manufacturer=@Manufacturer, manufacturer_specific_id=@ManufacturerSpecificId, type=@Type WHERE id=@Id",
                    param: new
                    {
                        material.Id,
                        material.Name,
                        material.Manufacturer,
                        material.ManufacturerSpecificId,
                        Type = material.Type.Id
                    });
            }
        }

        #region Private helpers

        /// <summary>
        /// A generic getter of materials. Gets all materials if called with (null,null).
        /// </summary>
        /// <param name="whereClause">An optional where clause.</param>
        /// <param name="dataParam">The values to fill into the where clause.</param>
        /// <returns>Returns all matching materials</returns>
        private IEnumerable<Material> GetMaterials(string whereClause, object dataParam)
        {
            string sql = "SELECT m.id, m.name, m.manufacturer, m.manufacturer_specific_id, p.id, p.name, t.material_id, t.prop_id, t.value, f.material_id, f.prop_id, f.file_path AS value";
            sql += " FROM materials m";
            sql += " JOIN plastics p ON p.id = m.type";
            sql += " LEFT JOIN text_material_prop_values t ON t.material_id = m.id";
            sql += " LEFT JOIN file_material_prop_values f ON f.material_id = m.id";
            if (!string.IsNullOrWhiteSpace(whereClause))
            {
                sql += " " + whereClause;
            }
            Dictionary<int, Material> materialMap = new Dictionary<int, Material>();

            IEnumerable<Material> materials = null;
            using (IDbConnection connection = GetNewConnection())
            {
                materials = connection.Query<Material, Plastic, CustomMaterialPropValue, CustomMaterialPropValue, Material>(sql,
                    (mat, plastic, textValue, fileValue) =>
                    {
                        Material material = null;
                        if (!materialMap.TryGetValue(mat.Id, out material))
                        {
                            material = mat;
                            materialMap.Add(material.Id, material);
                        }
                        material.Type = plastic;
                        if (textValue != null && !material.CustomProps.Any(p => p.PropId == textValue.PropId))
                        {
                            material.CustomProps.Add(textValue);
                        }
                        if (fileValue != null && !material.CustomProps.Any(p => p.PropId == fileValue.PropId))
                        {
                            material.CustomProps.Add(fileValue);
                        }
                        return material;
                    },
                    splitOn: "id,material_id,material_id",
                    param: dataParam);
            }
            return materials.Distinct();
        }

        #endregion
    }
}
