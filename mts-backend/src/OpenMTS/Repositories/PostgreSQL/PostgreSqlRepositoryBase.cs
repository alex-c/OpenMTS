using Microsoft.Extensions.Configuration;
using Npgsql;
using System.Data;

namespace OpenMTS.Repositories.PostgreSQL
{
    public class PostgreSqlRepositoryBase
    {
        /// <summary>
        /// Connection string for the underlying database.
        /// </summary>
        private string ConnectionString { get; }

        public PostgreSqlRepositoryBase(IConfiguration configuration)
        {
            ConnectionString = configuration.GetValue<string>("Database:ConnectionString");
        }

        /// <summary>
        /// Gets a new PostgreSQL connection.
        /// </summary>
        /// <returns>Returns a new connection.</returns>
        internal IDbConnection GetNewConnection()
        {
            return new NpgsqlConnection(ConnectionString);
        }
    }
}
