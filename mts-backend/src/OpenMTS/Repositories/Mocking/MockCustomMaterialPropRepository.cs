using OpenMTS.Models;
using System;
using System.Collections.Generic;

namespace OpenMTS.Repositories.Mocking
{
    public class MockCustomMaterialPropRepository : ICustomMaterialPropRepository
    {
        private Dictionary<Guid, CustomMaterialProp> Props { get; }

        public MockCustomMaterialPropRepository()
        {
            Props = new Dictionary<Guid, CustomMaterialProp>();
        }

        public IEnumerable<CustomMaterialProp> GetAllCustomMaterialProps()
        {
            return Props.Values;
        }

        public CustomMaterialProp GetCustomMaterialProp(Guid id)
        {
            return Props.GetValueOrDefault(id);
        }

        public CustomMaterialProp CreateCustomMaterialProp(string name, PropType type)
        {
            CustomMaterialProp prop = new CustomMaterialProp()
            {
                Id = Guid.NewGuid(),
                Name = name,
                Type = type
            };
            Props.Add(prop.Id, prop);
            return prop;
        }

        public void UpdateCustomMaterialProp(CustomMaterialProp prop)
        {
            Props[prop.Id] = prop;
        }

        public void DeleteCustomMaterialProp(Guid id)
        {
            Props.Remove(id);
        }
    }
}
