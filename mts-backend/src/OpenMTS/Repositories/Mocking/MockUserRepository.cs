using OpenMTS.Models;
using System.Collections.Generic;
using System.Linq;

namespace OpenMTS.Repositories.Mocking
{
    public class MockUserRepository : IUserRepository, IReadOnlyUserRepository
    {
        private Dictionary<string, User> Users { get; }

        public MockUserRepository(MockDataProvider dataProvider = null)
        {
            if (dataProvider == null)
            {
                Users = new Dictionary<string, User>();
            }
            else
            {
                Users = dataProvider.Users;
            }
        }

        public IEnumerable<User> GetAllUsers(bool showArchived)
        {
            IEnumerable<User> users;
            if (showArchived)
            {
                users = Users.Values;
            }
            else
            {
                users = Users.Values.Where(u => u.IsArchived == false);
            }
            return users;
        }

        public IEnumerable<User> SearchUsersByName(string partialName, bool showArchived)
        {
            IEnumerable<User> users;
            if (showArchived)
            {
                users = Users.Values;
            }
            else
            {
                users = Users.Values.Where(u => u.IsArchived == false);
            }
            return users.Where(u => u.Name.ToLowerInvariant().Contains(partialName));
        }

        public User GetUser(string id)
        {
            return Users.GetValueOrDefault(id);
        }

        public User CreateUser(string id, string name, string password, byte[] salt, Role role)
        {
            User user = new User()
            {
                Id = id,
                Name = name,
                Password = password,
                Salt = salt,
                Role = role,
                IsArchived = false
            };
            Users.Add(id, user);
            return user;
        }

        public void UpdateUser(User user)
        {
            Users[user.Id] = user;
        }

        public void DeleteUser(string id)
        {
            Users.Remove(id);
        }
    }
}
