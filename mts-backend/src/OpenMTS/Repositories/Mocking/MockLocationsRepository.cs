using OpenMTS.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OpenMTS.Repositories.Mocking
{
    public class MockLocationsRepository : ILocationsRepository
    {
        private Dictionary<Guid, StorageSite> StorageSites { get; }

        private Dictionary<Guid, StorageArea> StorageAreas { get; }

        public MockLocationsRepository(MockDataProvider dataProvider = null)
        {
            StorageAreas = new Dictionary<Guid, StorageArea>();
            if (dataProvider == null)
            {
                StorageSites = new Dictionary<Guid, StorageSite>();
            }
            else
            {
                StorageSites = dataProvider.StorageSites;
                foreach (StorageSite site in StorageSites.Values)
                {
                    foreach (StorageArea area in site.Areas)
                    {
                        StorageAreas.Add(area.Id, area);
                    }
                }
            }
        }

        public IEnumerable<StorageSite> GetAllStorageSites()
        {
            return StorageSites.Values;
        }

        public IEnumerable<StorageSite> SearchStorageSitesByName(string partialName)
        {
            return StorageSites.Values.Where(s =>s.Name.ToLowerInvariant().Contains(partialName.ToLowerInvariant()));
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

        public StorageArea CreateStorageArea(StorageSite storageSite, string areaName)
        {
            StorageArea area = new StorageArea(areaName);
            storageSite.Areas.Add(area);
            StorageAreas.Add(area.Id, area);
            StorageSites[storageSite.Id] = storageSite;
            return area;
        }

        public StorageArea UpdateStorageArea(StorageArea storageArea)
        {
            StorageAreas[storageArea.Id] = storageArea;
            return storageArea;
        }

        public void DeleteStorageSite(Guid id)
        {
            StorageSites.Remove(id);
        }
    }
}
