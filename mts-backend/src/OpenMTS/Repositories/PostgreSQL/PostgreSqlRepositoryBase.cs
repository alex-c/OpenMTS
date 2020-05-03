using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
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

        /// <summary>
        /// A logger for repository-level logging.
        /// </summary>
        protected ILogger Logger { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="PostgreSqlRepositoryBase"/> class.
        /// </summary>
        /// <param name="configuration">The configuration to get a connection string from.</param>
        /// <param name="logger">A logger for repository-level logging needs.</param>
        public PostgreSqlRepositoryBase(IConfiguration configuration, ILogger logger)
        {
            ConnectionString = configuration.GetValue<string>("Database:ConnectionString");
            Logger = logger;
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
