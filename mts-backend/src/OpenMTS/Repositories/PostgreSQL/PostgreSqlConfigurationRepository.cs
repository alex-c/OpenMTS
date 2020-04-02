using Dapper;
using Microsoft.Extensions.Configuration;
using Npgsql;
using OpenMTS.Models;
using System;
using System.Data;

namespace OpenMTS.Repositories.PostgreSQL
{
    public class PostgreSqlConfigurationRepository : PostgreSqlRepositoryBase, IConfigurationRepository
    {
        /// <summary>
        /// Sets up a PostgreSQL-based configuration repository from the app configuration.
        /// </summary>
        /// <param name="configuration">Application configuration.</param>
        public PostgreSqlConfigurationRepository(IConfiguration configuration) : base(configuration) { }

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
