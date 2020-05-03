using Dapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using OpenMTS.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace OpenMTS.Repositories.PostgreSQL
{
    /// <summary>
    /// A PostgreSQL-implementation of the API keys repository.
    /// </summary>
    /// <seealso cref="OpenMTS.Repositories.PostgreSQL.PostgreSqlRepositoryBase" />
    /// <seealso cref="OpenMTS.Repositories.IApiKeyRepository" />
    public class PostgreSqlApiKeyRepository : PostgreSqlRepositoryBase, IApiKeyRepository, IReadOnlyApiKeyRepository
    {
        private readonly string SQL_GET_API_KEYS = "SELECT * FROM api_keys LEFT JOIN api_keys_rights ON api_keys_rights.api_key_id = api_keys.id";
        private readonly string SQL_GET_API_KEY = "SELECT * FROM api_keys LEFT JOIN api_keys_rights ON api_keys_rights.api_key_id = api_keys.id WHERE id=@Id";

        /// <summary>
        /// Initializes a new instance of the <see cref="PostgreSqlApiKeyRepository"/> class.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        public PostgreSqlApiKeyRepository(IConfiguration configuration, ILogger<PostgreSqlApiKeyRepository> logger) : base(configuration, logger) { }

        /// <summary>
        /// Gets all available API keys.
        /// </summary>
        /// <returns>
        /// Returns all API keys.
        /// </returns>
        public IEnumerable<ApiKey> GetAllApiKeys()
        {
            IEnumerable<ApiKey> apiKeys = null;
            using (IDbConnection connection = GetNewConnection())
            {
                apiKeys = GetApiKeys(connection);
            }
            return apiKeys;
        }

        /// <summary>
        /// Gets an API key by it's ID.
        /// </summary>
        /// <param name="id">ID of the key to get.</param>
        /// <returns>
        /// Returns the API key if found, else null.
        /// </returns>
        public ApiKey GetApiKey(Guid id)
        {
            IEnumerable<ApiKey> apiKeys = null;
            using (IDbConnection connection = GetNewConnection())
            {
                apiKeys = GetApiKeys(connection, id);
            }
            return apiKeys.FirstOrDefault();
        }

        /// <summary>
        /// Creates a new API key.
        /// </summary>
        /// <param name="name">Name of the API key to create.</param>
        /// <returns>
        /// Returns the newly crated API key.
        /// </returns>
        public ApiKey CreateApiKey(string name)
        {
            Guid id = Guid.NewGuid();

            ApiKey key = null;
            using (IDbConnection connection = GetNewConnection())
            {
                connection.Execute("INSERT INTO api_keys (id, name) VALUES (@Id, @Name)", new
                {
                    Id = id,
                    Name = name
                });
                key = GetApiKeys(connection, id).First();
            }
            return key;
        }

        /// <summary>
        /// Updates an existing API key.
        /// </summary>
        /// <param name="apiKey">key to update.</param>
        public void UpdateApiKey(ApiKey apiKey)
        {
            using (IDbConnection connection = GetNewConnection())
            {
                connection.Execute("UPDATE api_keys SET name=@Name, enabled=@Enabled WHERE id=@Id", new { apiKey.Id, apiKey.Name, apiKey.Enabled });
                connection.Execute("DELETE FROM api_keys_rights WHERE api_key_id=@Id", new { apiKey.Id });
                connection.Execute("INSERT INTO api_keys_rights (api_key_id, right_id) VALUES (@KeyId, @RightId)",
                    apiKey.Rights.Select(right => new { KeyId = apiKey.Id, RightId = right.Id }));
            }
        }

        /// <summary>
        /// Deletes an API key.
        /// </summary>
        /// <param name="id">ID of the key to delete.</param>
        public void DeleteApiKey(Guid id)
        {
            using (IDbConnection connection = GetNewConnection())
            {
                connection.Execute("DELETE FROM api_keys_rights WHERE api_key_id=@Id", new { id });
                connection.Execute("DELETE FROM api_keys WHERE id=@Id", new { id });
            }
        }

        #region Private helpers

        /// <summary>
        /// Gets an API key list, maybe filtered by an ID.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <param name="id">Optional key ID to filter with.</param>
        /// <returns>Returns a list of keys.</returns>
        private IEnumerable<ApiKey> GetApiKeys(IDbConnection connection, Guid? id = null)
        {
            Dictionary<Guid, ApiKey> keys = new Dictionary<Guid, ApiKey>();
            string sql = id != null ? SQL_GET_API_KEY : SQL_GET_API_KEYS;
            return connection.Query<ApiKey, ApiKeyRights, ApiKey>(sql,
                map: (apiKey, keyRights) =>
                {
                    ApiKey key = null;
                    if (!keys.TryGetValue(apiKey.Id, out key))
                    {
                        key = apiKey;
                        keys.Add(key.Id, key);
                    }
                    if (keyRights != null)
                    {
                        key.Rights.Add(new Right(keyRights.RightId));
                    }
                    return key;
                },
                splitOn: "api_key_id",
                param: id != null ? new { id } : null).Distinct();
        }

        /// <summary>
        /// Model for the `api_keys_rights` table.
        /// </summary>
        internal class ApiKeyRights
        {
            public Guid ApiKeyId { get; set; }
            public string RightId { get; set; }
        }

        #endregion
    }
}
