using Dapper;
using Microsoft.Extensions.Configuration;
using Npgsql;
using OpenMTS.Models;
using System;
using System.Collections.Generic;
using System.Data;

namespace OpenMTS.Repositories.PostgreSQL
{
    /// <summary>
    /// A user repository based on a PostrgreSQL database.
    /// </summary>
    /// <seealso cref="OpenMTS.Repositories.IUserRepository" />
    public class PostgreSqlUserRepository : IUserRepository
    {
        /// <summary>
        /// Connection string for the underlying database.
        /// </summary>
        private string ConnectionString { get; }

        /// <summary>
        /// Sets up a PostgreSQL-based user repository from the app configuration.
        /// </summary>
        /// <param name="configuration">Application configuration.</param>
        public PostgreSqlUserRepository(IConfiguration configuration)
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

        public IEnumerable<User> GetAllUsers(bool showDisabled = false)
        {
            IEnumerable<User> users = null;
            using (IDbConnection connection = GetNewConnection())
            {
                if (showDisabled)
                {
                    users = connection.Query<User>("SELECT * FROM users");
                }
                else
                {
                    users = connection.Query<User>("SELECT * FROM users WHERE disabled=false");
                }
            }
            return users;
        }

        public IEnumerable<User> SearchUsersByName(string partialName, bool showDisabled)
        {
            partialName = $"%{partialName}%";
            IEnumerable<User> users = null;
            using (IDbConnection connection = GetNewConnection())
            {
                if (showDisabled)
                {
                    users = connection.Query<User>("SELECT * FROM users WHERE name LIKE @Name", new { partialName });
                }
                else
                {
                    users = connection.Query<User>("SELECT * FROM users WHERE disabled=false AND name LIKE @Name", new { partialName });
                }
            }
            return users;
        }

        public User GetUser(string id)
        {
            User user = null;
            using (IDbConnection connection = GetNewConnection())
            {
                user = connection.QuerySingle<User>("SELECT * FROM users WHERE id=@Id", new { id });
            }
            return user;
        }

        public User CreateUser(string id, string name, string password, byte[] salt, Role role)
        {
            User user = null;
            using (IDbConnection connection = GetNewConnection())
            {
                connection.Execute("INSERT INTO users (id, name, password, salt, role) VALUES (@Id, @Name, @Password, @Salt, @Role)", new
                {
                    Id = id,
                    Name = name,
                    Password = password,
                    Salt = salt,
                    Role = role
                });
                user = connection.QuerySingle<User>("SELECT * FROM users WHERE id=@Id", new { id });
            }
            return user;
        }

        public void UpdateUser(User user)
        {
            using (IDbConnection connection = GetNewConnection())
            {
                connection.Execute("UPDATE users SET name=@Name, password=@Password, salt=@Salt, role=@Role", new
                {
                    user.Name,
                    user.Password,
                    user.Salt,
                    user.Role
                });
            }
        }

        public void DeleteUser(string id)
        {
            using (IDbConnection connection = GetNewConnection())
            {
                connection.Execute("DELETE FROM users WHERE id=@Id", new { Id = id });
            }
        }
    }
}
