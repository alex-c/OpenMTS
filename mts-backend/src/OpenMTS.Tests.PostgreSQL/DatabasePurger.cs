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
        /// Resets the OpenMTS configuration to default values.
        /// </summary>
        public static void ResetConfiguration()
        {
            using (IDbConnection connection = new NpgsqlConnection(GetConnectionString()))
            {
                connection.Execute("UPDATE configuration SET allow_guest_login=false");
            }
        }

        /// <summary>
        /// Purges all API keys.
        /// </summary>
        public static void PurgeApiKeys()
        {
            using (IDbConnection connection = new NpgsqlConnection(GetConnectionString()))
            {
                connection.Execute("DELETE FROM api_keys_rights");
                connection.Execute("DELETE FROM api_keys");
            }
        }

        /// <summary>
        /// Purges all users.
        /// </summary>
        public static void PurgeUsers()
        {
            using (IDbConnection connection = new NpgsqlConnection(GetConnectionString()))
            {
                connection.Execute("DELETE FROM users");
            }
        }

        /// <summary>
        /// Purges all plastics.
        /// </summary>
        public static void PurgePlastics()
        {
            using (IDbConnection connection = new NpgsqlConnection(GetConnectionString()))
            {
                connection.Execute("DELETE FROM plastics");
            }
        }

        /// <summary>
        /// Purges all storage sites and areas.
        /// </summary>
        public static void PurgeLocations()
        {
            using (IDbConnection connection = new NpgsqlConnection(GetConnectionString()))
            {
                connection.Execute("DELETE FROM storage_areas");
                connection.Execute("DELETE FROM storage_sites");
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
