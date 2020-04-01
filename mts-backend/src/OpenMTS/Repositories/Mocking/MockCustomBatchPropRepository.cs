using OpenMTS.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace OpenMTS.Repositories.Mocking
{
    public class MockCustomBatchPropRepository : ICustomBatchPropRepository
    {
        private Dictionary<Guid, CustomBatchProp> Props { get; }

        public MockCustomBatchPropRepository(MockDataProvider mockDataProvider = null)
        {
            if (mockDataProvider == null)
            {
                Props = new Dictionary<Guid, CustomBatchProp>();
            }
            else
            {
                Props = mockDataProvider.CustomBatchProps;
            }
        }
        public IEnumerable<CustomBatchProp> GetAllCustomBatchProps()
        {
            return Props.Values;
        }

        public CustomBatchProp GetCustomBatchProp(Guid id)
        {
            return Props.GetValueOrDefault(id);
        }

        public CustomBatchProp CreateCustomBatchProp(string name)
        {
            CustomBatchProp prop = new CustomBatchProp()
            {
                Id = Guid.NewGuid(),
                Name = name
            };
            Props.Add(prop.Id, prop);
            return prop;
        }

        public void UpdateCustomBatchProp(CustomBatchProp prop)
        {
            Props[prop.Id] = prop;
        }

        public void DeleteCustomBatchProp(Guid id)
        {
            Props.Remove(id);
        }
    }
}
