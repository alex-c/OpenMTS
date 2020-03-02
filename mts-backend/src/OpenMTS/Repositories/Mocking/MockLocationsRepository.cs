using OpenMTS.Models;
using System;
using System.Collections.Generic;

namespace OpenMTS.Repositories.Mocking
{
    public class MockLocationsRepository : ILocationsRepository
    {
        private Dictionary<Guid, StorageSite> StorageSites { get; }

        public MockLocationsRepository(MockDataProvider dataProvider = null)
        {
            if (dataProvider == null)
            {
                StorageSites = new Dictionary<Guid, StorageSite>();
            }
            else
            {
                StorageSites = dataProvider.StorageSites;
            }
        }

        public IEnumerable<StorageSite> GetAllStorageSites()
        {
            return StorageSites.Values;
        }

        public StorageSite GetStorageSite(Guid id)
        {
            return StorageSites.GetValueOrDefault(id);
        }

        public StorageSite CreateStorageSite(string name)
        {
            StorageSite site = new StorageSite()
            {
                Id = Guid.NewGuid(),
                Name = name,
                Areas = new List<StorageArea>()
            };
            StorageSites.Add(site.Id, site);
            return site;
        }

        public StorageSite UpdateStorageSite(StorageSite storageSite)
        {
            StorageSites[storageSite.Id] = storageSite;
            return storageSite;
        }

        public void DeleteStorageSite(Guid id)
        {
            StorageSites.Remove(id);
        }
    }
}
