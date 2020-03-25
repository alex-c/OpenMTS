using OpenMTS.Models;
using System.Collections.Generic;
using System.Linq;

namespace OpenMTS.Repositories.Mocking
{
    public class MockPlasticsRepository : IPlasticsRepository
    {
        private Dictionary<string, Plastic> Plastics { get; }

        public MockPlasticsRepository(MockDataProvider mockDataProvider = null)
        {
            if (mockDataProvider == null)
            {
                Plastics = new Dictionary<string, Plastic>();
            }
            else
            {
                Plastics = mockDataProvider.Plastics;
            }
        }

        public IEnumerable<Plastic> GetAllPlastics()
        {
            return Plastics.Values;
        }

        public IEnumerable<Plastic> SearchPlasticsByName(string partialName)
        {
            return Plastics.Values.Where(m => m.Name.ToLowerInvariant().Contains(partialName.ToLowerInvariant()));
        }

        public Plastic GetPlastic(string id)
        {
            return Plastics.GetValueOrDefault(id);
        }

        public Plastic CreatePlastic(string id, string name)
        {
            Plastic plastic = new Plastic() { Id = id, Name = name };
            Plastics.Add(id, plastic);
            return plastic;
        }

        public void UpdatePlastic(Plastic plastic)
        {
            Plastics[plastic.Id] = plastic;
        }

        public void DeletePlastic(string id)
        {
            Plastics.Remove(id);
        }
    }
}
