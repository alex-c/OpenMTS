using Dapper;
using Microsoft.Extensions.Configuration;
using Npgsql;
using System.Data;

namespace OpenMTS.Tests.PostgreSQL
{
    /// <summary>
    /// Utility to purge database tables before and after executing tests.
    /// </summary>
    public static class DatabasePurger
    {
        /// <summary>
        /// Purges all users.
        /// </summary>
        public static void PurgeUsers()
        {
            string connectionString = GetConnectionString();
            using (IDbConnection connection = new NpgsqlConnection(connectionString))
            {
                connection.Execute("DELETE FROM users");
            }
        }

        /// <summary>
        /// Gets the dataabase connection string from configuration.
        /// </summary>
        /// <returns>Returns the connection string.</returns>
        private static string GetConnectionString()
        {
            return ConfigurationProvider.GetConfiguration().GetValue<string>("Database:ConnectionString");
        }
    }
}
