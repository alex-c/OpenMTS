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
    public class PostgreSqlUserRepository : PostgreSqlRepositoryBase, IUserRepository
    {
        /// <summary>
        /// Sets up a PostgreSQL-based user repository from the app configuration.
        /// </summary>
        /// <param name="configuration">Application configuration.</param>
        public PostgreSqlUserRepository(IConfiguration configuration) : base(configuration) { }

        /// <summary>
        /// Gets all users.
        /// </summary>
        /// <param name="showDisabled">Whether to return soft-deleted users.</param>
        /// <returns>
        /// Returns all users.
        /// </returns>
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

        /// <summary>
        /// Searches users by name using a partial name.
        /// </summary>
        /// <param name="partialName">Partial name to search for.</param>
        /// <param name="showDisabled">Whether to return soft-deleted users.</param>
        /// <returns>
        /// Returns a list of matching users.
        /// </returns>
        public IEnumerable<User> SearchUsersByName(string partialName, bool showDisabled)
        {
            partialName = $"%{partialName}%";
            IEnumerable<User> users = null;
            using (IDbConnection connection = GetNewConnection())
            {
                if (showDisabled)
                {
                    users = connection.Query<User>("SELECT * FROM users WHERE name ILIKE @Name", new { Name = partialName });
                }
                else
                {
                    users = connection.Query<User>("SELECT * FROM users WHERE disabled=false AND name ILIKE @Name", new { Name = partialName });
                }
            }
            return users;
        }

        /// <summary>
        /// Gets a user by his unique user ID.
        /// </summary>
        /// <param name="id">ID of the user.</param>
        /// <returns>
        /// Returns the user, or null if no matching user was found.
        /// </returns>
        public User GetUser(string id)
        {
            User user = null;
            using (IDbConnection connection = GetNewConnection())
            {
                user = connection.QuerySingle<User>("SELECT * FROM users WHERE id=@Id", new { id });
            }
            return user;
        }

        /// <summary>
        /// Creates a new user.
        /// </summary>
        /// <param name="id">The ID of the user to create.</param>
        /// <param name="name">The display name of the user to create.</param>
        /// <param name="password">The password of the user to create.</param>
        /// <param name="salt">The salt used to had the user's password.</param>
        /// <param name="role">The user role to assigne to the user to create.</param>
        /// <returns>
        /// Returns the newly created user.
        /// </returns>
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

        /// <summary>
        /// Updates a user.
        /// </summary>
        /// <param name="user">The user top update.</param>
        public void UpdateUser(User user)
        {
            using (IDbConnection connection = GetNewConnection())
            {
                connection.Execute("UPDATE users SET name=@Name, password=@Password, salt=@Salt, role=@Role, disabled=@Disabled WHERE id=@Id", new
                {
                    user.Id,
                    user.Name,
                    user.Password,
                    user.Salt,
                    user.Role,
                    user.Disabled
                });
            }
        }

        /// <summary>
        /// Deletes a user, identified by his unique ID.
        /// </summary>
        /// <param name="id">The ID of the user to delete.</param>
        public void DeleteUser(string id)
        {
            using (IDbConnection connection = GetNewConnection())
            {
                connection.Execute("DELETE FROM users WHERE id=@Id", new { Id = id });
            }
        }
    }
}
