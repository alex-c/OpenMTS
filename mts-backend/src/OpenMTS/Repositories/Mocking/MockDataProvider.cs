using OpenMTS.Models;
using OpenMTS.Services;
using System;
using System.Collections.Generic;

namespace OpenMTS.Repositories.Mocking
{
    public class MockDataProvider
    {
        public Dictionary<string, User> Users { get; }

        public Dictionary<Guid, StorageSite> StorageSites { get; }

        public MockDataProvider(PasswordHashingService passwordHashingService)
        {
            #region Users

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

            #endregion

            #region Locations

            StorageSites = new Dictionary<Guid, StorageSite>();

            Guid id = Guid.NewGuid();
            StorageSites.Add(id, new StorageSite()
            {
                Id = id,
                Name = "Pontstr. Keller",
                Areas = new List<StorageArea>()
                {
                    new StorageArea("Regal links"),
                    new StorageArea("Palette rechts"),
                    new StorageArea("Palette hinten")
                }
            });

            id = Guid.NewGuid();
            StorageSites.Add(id, new StorageSite()
            {
                Id = id,
                Name = "Pontstr. Empore Maschinenhalle",
                Areas = new List<StorageArea>()
                {
                    new StorageArea("Abstellplatz links"),
                    new StorageArea("Abstellplatz rechts")
                }
            });

            #endregion
        }
    }
}
