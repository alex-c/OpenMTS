using Dapper;
using Microsoft.Extensions.Configuration;
using OpenMTS.Models;
using System.Collections.Generic;
using System.Data;

namespace OpenMTS.Repositories.PostgreSQL
{
    /// <summary>
    /// A PostgreSQL implementation of the plastics repository.
    /// </summary>
    /// <seealso cref="OpenMTS.Repositories.PostgreSQL.PostgreSqlRepositoryBase" />
    /// <seealso cref="OpenMTS.Repositories.IPlasticsRepository" />
    public class PostgreSqlPlasticsRepository : PostgreSqlRepositoryBase, IPlasticsRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PostgreSqlPlasticsRepository"/> class.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        public PostgreSqlPlasticsRepository(IConfiguration configuration) : base(configuration) { }

        /// <summary>
        /// Gets all available plastics.
        /// </summary>
        /// <returns>
        /// Returns all plastics.
        /// </returns>
        public IEnumerable<Plastic> GetAllPlastics()
        {
            IEnumerable<Plastic> plastics = null;
            using (IDbConnection connection = GetNewConnection())
            {
                plastics = connection.Query<Plastic>("SELECT * FROM plastics");
            }
            return plastics;
        }

        /// <summary>
        /// Gets and filters plastics using a partial name.
        /// </summary>
        /// <param name="partialName">String to filter with..</param>
        /// <returns>
        /// Returns filtered plastics.
        /// </returns>
        public IEnumerable<Plastic> SearchPlasticsByName(string partialName)
        {
            string name = $"%{partialName}%";
            IEnumerable<Plastic> plastics = null;
            using (IDbConnection connection = GetNewConnection())
            {
                plastics = connection.Query<Plastic>("SELECT * FROM plastics WHERE name ILIKE @Name", new { name });
            }
            return plastics;
        }

        /// <summary>
        /// Gets a plastic.
        /// </summary>
        /// <param name="id">The ID of the plastic to get.</param>
        /// <returns>
        /// Returns the plastic or null.
        /// </returns>
        public Plastic GetPlastic(string id)
        {
            Plastic plastic = null;
            using (IDbConnection connection = GetNewConnection())
            {
                plastic = connection.QuerySingle<Plastic>("SELECT * FROM plastics WHERE id=@Id", new { id });
            }
            return plastic;
        }

        /// <summary>
        /// Creates a new plastic.
        /// </summary>
        /// <param name="id">The ID of the new plastic.</param>
        /// <param name="name">The name of the plastic to create.</param>
        /// <returns>
        /// Returns the newly created plastic.
        /// </returns>
        public Plastic CreatePlastic(string id, string name)
        {
            Plastic plastic = null;
            using (IDbConnection connection = GetNewConnection())
            {
                connection.Execute("INSERT INTO plastics (id, name) VALUES (@Id, @Name)", new { id, name });
                plastic = connection.QuerySingleOrDefault<Plastic>("SELECT * FROM plastics WHERE id=@Id", new { id });
            }
            return plastic;
        }

        /// <summary>
        /// Updates a plastic.
        /// </summary>
        /// <param name="plastic">The plastic to update.</param>
        public void UpdatePlastic(Plastic plastic)
        {
            using (IDbConnection connection = GetNewConnection())
            {
                connection.Execute("UPDATE plastics SET name=@Name WHERE id=@Id", new
                {
                    plastic.Id,
                    plastic.Name,
                });
            }
        }

        /// <summary>
        /// Deletes a plastic.
        /// </summary>
        /// <param name="id">The ID of the plastic to delete.</param>
        public void DeletePlastic(string id)
        {
            using (IDbConnection connection = GetNewConnection())
            {
                connection.Execute("DELETE FROM plastics WHERE id=@Id", new { id });
            }
        }
    }
}
