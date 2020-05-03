using Dapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using OpenMTS.Models;
using System;
using System.Collections.Generic;
using System.Data;

namespace OpenMTS.Repositories.PostgreSQL
{
    /// <summary>
    /// A PostgreSQL implementation of the custom material props repository.
    /// </summary>
    /// <seealso cref="OpenMTS.Repositories.PostgreSQL.PostgreSqlRepositoryBase" />
    /// <seealso cref="OpenMTS.Repositories.IMaterialsRepository" />
    public class PostgreSqlCustomMaterialPropRepository : PostgreSqlRepositoryBase, ICustomMaterialPropRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PostgreSqlCustomMaterialPropRepository"/> class.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        public PostgreSqlCustomMaterialPropRepository(IConfiguration configuration, ILogger<PostgreSqlCustomMaterialPropRepository> logger) : base(configuration, logger) { }

        /// <summary>
        /// Gets all custom material properties.
        /// </summary>
        /// <returns>
        /// Returns a list of props.
        /// </returns>
        public IEnumerable<CustomMaterialProp> GetAllCustomMaterialProps()
        {
            IEnumerable<CustomMaterialProp> props = null;
            using (IDbConnection connection = GetNewConnection())
            {
                props = connection.Query<CustomMaterialProp>("SELECT * FROM material_props");
            }
            return props;
        }

        /// <summary>
        /// Gets a custom material property.
        /// </summary>
        /// <param name="id">The property's ID.</param>
        /// <returns>
        /// Returns the property or null.
        /// </returns>
        public CustomMaterialProp GetCustomMaterialProp(Guid id)
        {
            CustomMaterialProp prop = null;
            using (IDbConnection connection = GetNewConnection())
            {
                prop = connection.QuerySingle<CustomMaterialProp>("SELECT * FROM material_props WHERE id=@Id", new { id });
            }
            return prop;
        }

        /// <summary>
        /// Creates a custom material property.
        /// </summary>
        /// <param name="name">The property's name.</param>
        /// <param name="type">The property's type.</param>
        /// <returns>
        /// Returns the newly created prop.
        /// </returns>
        public CustomMaterialProp CreateCustomMaterialProp(string name, PropType type)
        {
            Guid id = Guid.NewGuid();

            CustomMaterialProp prop = null;
            using (IDbConnection connection = GetNewConnection())
            {
                connection.Execute("INSERT INTO material_props (id, name, type) VALUES (@Id, @Name, @Type)",
                    param: new
                    {
                        id,
                        name,
                        type
                    });
                prop = connection.QuerySingle<CustomMaterialProp>("SELECT * FROM material_props WHERE id=@Id", param: new { id });
            }
            return prop;
        }

        /// <summary>
        /// Updates a custom material property.
        /// </summary>
        /// <param name="prop">Prop to update.</param>
        public void UpdateCustomMaterialProp(CustomMaterialProp prop)
        {
            using(IDbConnection connection = GetNewConnection())
            {
                connection.Execute("UPDATE material_props SET name=@Name, type=@Type WHERE id=@Id",
                    param: new
                    {
                        prop.Id,
                        prop.Name,
                        prop.Type
                    });
            }
        }

        /// <summary>
        /// Deletes a custom material property.
        /// </summary>
        /// <param name="id">The ID of the prop to delete.</param>
        public void DeleteCustomMaterialProp(Guid id)
        {
            using (IDbConnection connection = GetNewConnection())
            {
                connection.Execute("DELETE FROM file_material_prop_values WHERE prop_id=@Id", new { id });
                connection.Execute("DELETE FROM text_material_prop_values WHERE prop_id=@Id", new { id });
                connection.Execute("DELETE FROM material_props WHERE id=@Id", new { id });
            }
        }
    }
}
