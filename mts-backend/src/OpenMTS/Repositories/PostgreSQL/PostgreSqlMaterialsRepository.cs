using Dapper;
using Microsoft.Extensions.Configuration;
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
        public PostgreSqlMaterialsRepository(IConfiguration configuration) : base(configuration) { }

        /// <summary>
        /// Gets all materials.
        /// </summary>
        /// <returns>
        /// Returns all materials
        /// </returns>
        public IEnumerable<Material> GetAllMaterials()
        {
            string sql = "SELECT m.id, m.name, m.manufacturer, m.manufacturer_specific_id, p.id, p.name FROM materials m JOIN plastics p ON p.id = m.type";

            IEnumerable<Material> materials = null;
            using (IDbConnection connection = GetNewConnection())
            {
                // TODO: join custom prop values
                materials = connection.Query<Material, Plastic, Material>(sql,
                    (material, plastic) =>
                    {
                        material.Type = plastic;
                        return material;
                    }, splitOn: "id");
            }
            return materials;
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
            string sql = $"SELECT m.id, m.name, m.manufacturer, m.manufacturer_specific_id, p.id, p.name FROM materials m JOIN plastics p ON p.id = m.type";

            // Build query
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
            sql += whereClause;

            // Query
            IEnumerable<Material> materials = null;
            using (IDbConnection connection = GetNewConnection())
            {
                // TODO: join custom prop values
                materials = connection.Query<Material, Plastic, Material>(sql, (material, plastic) =>
                {
                    material.Type = plastic;
                    return material;
                },
                splitOn: "id",
                param: new
                {
                    Name = partialName,
                    Manufacturer = partialManufacturerName,
                    Type = type?.Id
                });
            }
            return materials;
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
            string sql = "SELECT m.id, m.name, m.manufacturer, m.manufacturer_specific_id, p.id, p.name FROM materials m JOIN plastics p ON p.id = m.type WHERE m.id=@Id";

            IEnumerable<Material> materials = null;
            using (IDbConnection connection = GetNewConnection())
            {
                // TODO: join custom prop values
                materials = connection.Query<Material, Plastic, Material>(sql,
                    (material, plastic) =>
                    {
                        material.Type = plastic;
                        return material;
                    },
                    splitOn: "id",
                    param: new { id });
            }
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
                // Insert
                int id = connection.QuerySingle<int>("INSERT INTO materials (name, manufacturer, manufacturer_specific_id, type) VALUES (@Name, @Manufacturer, @ManufacturerSpecificId, @Type) RETURNING id",
                    param: new
                    {
                        Name = name,
                        Manufacturer = manufacturerName,
                        ManufacturerSpecificId = manufacturerSpecificId,
                        Type = type.Id
                    });

                // Get last inserted
                // TODO: join custom prop values
                materials = connection.Query<Material, Plastic, Material>("SELECT m.id, m.name, m.manufacturer, m.manufacturer_specific_id, p.id, p.name FROM materials m JOIN plastics p ON p.id = m.type WHERE m.id=@Id",
                    map: (mat, plastic) =>
                    {
                        mat.Type = plastic;
                        return mat;
                    },
                    splitOn: "id",
                    param: new { id });
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
                    param: new { 
                        material.Id,
                        material.Name,
                        material.Manufacturer,
                        material.ManufacturerSpecificId,
                        Type = material.Type.Id
                    });
            }
        }
    }
}
