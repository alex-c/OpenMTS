using OpenMTS.Models;
using OpenMTS.Services;
using System.Collections.Generic;

namespace OpenMTS.Repositories.Mocking
{
    public class MockDataProvider
    {
        public Dictionary<string, User> Users { get; }

        public string DefaultBlogId { get; } = "main";

        public MockDataProvider(PasswordHashingService passwordHashingService)
        {
            Users = new Dictionary<string, User>();

            (string hash, byte[] salt) = passwordHashingService.HashAndSaltPassword("test");
            User alex = new User()
            {
                Id = "alex",
                Password = hash,
                Salt = salt,
                Name = "Alexandre",
                Role = Role.Administrator
            };

            (hash, salt) = passwordHashingService.HashAndSaltPassword("test2");
            User anna = new User()
            {
                Id = "anna",
                Password = hash,
                Salt = salt,
                Name = "Anna M",
                Role = Role.User
            };

            Users.Add(alex.Id, alex);
            Users.Add(anna.Id, anna);
        }
    }
}
