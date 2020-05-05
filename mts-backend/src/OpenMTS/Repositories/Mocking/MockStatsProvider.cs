using OpenMTS.Models.Environmnt;
using OpenMTS.Models.Stats;
using System.Collections.Generic;
using System.Linq;

namespace OpenMTS.Repositories.Mocking
{
    public class MockStatsProvider : IStatsProvider
    {
        ILocationsRepository LocationsRepository { get; }

        IMaterialBatchRepository BatchRepository { get; }

        IEnvironmentalDataRepository EnvironmentalDataRepository { get; }

        public MockStatsProvider(ILocationsRepository locationsRepository, IMaterialBatchRepository batchRepository, IEnvironmentalDataRepository environmentalDataRepository)
        {
            LocationsRepository = locationsRepository;
            BatchRepository = batchRepository;
            EnvironmentalDataRepository = environmentalDataRepository;
        }
        public IEnumerable<StorageSiteOverview> GetSitesOverview()
        {
            List<StorageSiteOverview> overview = new List<StorageSiteOverview>();
            var sites = LocationsRepository.GetAllStorageSites();
            foreach (var site in sites)
            {
                overview.Add(new StorageSiteOverview()
                {
                    Site = site,
                    Temperature = EnvironmentalDataRepository.GetLatestValue(site, EnvironmentalFactor.Temperature).Value,
                    Humidity = EnvironmentalDataRepository.GetLatestValue(site, EnvironmentalFactor.Humidity).Value,
                    TotalMaterial = BatchRepository.GetMaterialBatches(null, site.Id).Sum(b => b.Quantity)
                });
            }
            return overview;
        }
    }
}
