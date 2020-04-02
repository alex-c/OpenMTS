using Dapper;
using Microsoft.Extensions.Configuration;
using Npgsql;
using OpenMTS.Models;
using System;
using System.Data;

namespace OpenMTS.Repositories.PostgreSQL
{
    public class PostgreSqlConfigurationRepository : IConfigurationRepository
    {
        /// <summary>
        /// Connection string for the underlying database.
        /// </summary>
        private string ConnectionString { get; }

        /// <summary>
        /// Sets up a PostgreSQL-based configuration repository from the app configuration.
        /// </summary>
        /// <param name="configuration">Application configuration.</param>
        public PostgreSqlConfigurationRepository(IConfiguration configuration)
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

        /// <summary>
        /// Gets the current OpenMTS configuration.
        /// </summary>
        /// <returns>
        /// Returns the current configuration.
        /// </returns>
        public Configuration GetConfiguration()
        {
            Configuration configuration = null;
            using (IDbConnection connection = GetNewConnection())
            {
                configuration = connection.QuerySingle<Configuration>("SELECT * FROM configuration");
            }
            return configuration;
        }

        /// <summary>
        /// Sets the current OpenMTS configuration.
        /// </summary>
        /// <param name="configuration">The configuration to set.</param>
        public void SetConfiguration(Configuration configuration)
        {
            using (IDbConnection connection = GetNewConnection())
            {
                connection.Execute("UPDATE configuration SET allowGuestLogin=@AllowGuestLogin", new { configuration.AllowGuestLogin });
            }
        }
    }
}
