using Dapper;
using Microsoft.Extensions.Configuration;
using OpenMTS.Models;
using System;
using System.Collections.Generic;
using System.Data;

namespace OpenMTS.Repositories.PostgreSQL
{
    /// <summary>
    /// A PostgreSQL implementation of the custom batch prop repository.
    /// </summary>
    /// <seealso cref="OpenMTS.Repositories.PostgreSQL.PostgreSqlRepositoryBase" />
    /// <seealso cref="OpenMTS.Repositories.ICustomBatchPropRepository" />
    public class PostgreSqlCustomBatchPropRepository : PostgreSqlRepositoryBase, ICustomBatchPropRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PostgreSqlCustomBatchPropRepository"/> class.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        public PostgreSqlCustomBatchPropRepository(IConfiguration configuration) : base(configuration) { }

        /// <summary>
        /// Gets all custom batch properties.
        /// </summary>
        /// <returns>
        /// Returns a list of props.
        /// </returns>
        /// <exception cref="NotImplementedException"></exception>
        public IEnumerable<CustomBatchProp> GetAllCustomBatchProps()
        {
            IEnumerable<CustomBatchProp> props = null;
            using (IDbConnection connection = GetNewConnection())
            {
                props = connection.Query<CustomBatchProp>("SELECT * FROM batch_props");
            }
            return props;
        }

        /// <summary>
        /// Gets a custom batch property.
        /// </summary>
        /// <param name="id">The property's ID.</param>
        /// <returns>
        /// Returns the property or null.
        /// </returns>
        /// <exception cref="NotImplementedException"></exception>
        public CustomBatchProp GetCustomBatchProp(Guid id)
        {
            CustomBatchProp prop = null;
            using (IDbConnection connection = GetNewConnection())
            {
                prop = connection.QuerySingleOrDefault<CustomBatchProp>("SELECT * FROM batch_props WHERE id=@Id", new { id });
            }
            return prop;
        }

        /// <summary>
        /// Creates a custom batch property.
        /// </summary>
        /// <param name="name">The property's name.</param>
        /// <returns>
        /// Returns the newly created prop.
        /// </returns>
        /// <exception cref="NotImplementedException"></exception>
        public CustomBatchProp CreateCustomBatchProp(string name)
        {
            Guid id = Guid.NewGuid();

            CustomBatchProp prop = null;
            using (IDbConnection connection = GetNewConnection())
            {
                connection.Execute("INSERT INTO batch_props (id, name) VALUES (@Id, @Name)", new { id, name });
                prop = connection.QuerySingle<CustomBatchProp>("SELECT * FROM batch_props WHERE id=@Id", new { id });
            }
            return prop;
        }

        /// <summary>
        /// Updates a custom batch property.
        /// </summary>
        /// <param name="prop">Prop to update.</param>
        /// <exception cref="NotImplementedException"></exception>
        public void UpdateCustomBatchProp(CustomBatchProp prop)
        {
            using (IDbConnection connection = GetNewConnection())
            {
                connection.Execute("UPDATE batch_props SET name=@Name WHERE id=@id", new { prop.Id, prop.Name });
            }
        }

        /// <summary>
        /// Deletes a custom batch property.
        /// </summary>
        /// <param name="id">The ID of the prop to delete.</param>
        /// <exception cref="NotImplementedException"></exception>
        public void DeleteCustomBatchProp(Guid id)
        {
            using (IDbConnection connection = GetNewConnection())
            {
                connection.Execute("DELETE FROM batch_prop_values WHERE prop_id=@Id", new { id });
                connection.Execute("DELETE FROM batch_props WHERE id=@Id", new { id });
            }
        }
    }
}
