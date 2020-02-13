using OpenMTS.Models;
using System.Collections.Generic;

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

        public User CreateUser(string id, string name, string password, byte[] salt)
        {
            User user = new User()
            {
                Id = id,
                Name = name,
                Password = password,
                Salt = salt
            };
            Users.Add(id, user);
            return user;
        }

        public void DeleteUser(string id)
        {
            Users.Remove(id);
        }

        public IEnumerable<User> GetAllUsers()
        {
            return Users.Values;
        }

        public User GetUser(string id)
        {
            return Users.GetValueOrDefault(id);
        }

        public void UpdateUser(User user)
        {
            Users[user.Id] = user;
        }
    }
}
