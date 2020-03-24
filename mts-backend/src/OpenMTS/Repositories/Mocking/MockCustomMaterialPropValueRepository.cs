using OpenMTS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenMTS.Repositories.Mocking
{
    public class MockCustomMaterialPropValueRepository : ICustomMaterialPropValueRepository
    {
        public List<CustomMaterialPropValue> PropValues { get; }

        public MockCustomMaterialPropValueRepository()
        {
            PropValues = new List<CustomMaterialPropValue>();
        }

        public void SetCustomTextMaterialProp(int materialId, Guid propId, string text)
        {
            CustomMaterialPropValue propValue = GetPropValue(materialId, propId);

            if (propValue == null)
            {
                PropValues.Add(new CustomMaterialPropValue()
                {
                    MaterialId = materialId,
                    PropId = propId,
                    Value = text
                });
            }
            else
            {
                propValue.Value = text;
            }
        }

        public void RemoveCustomTextMaterialProp(int materialId, Guid propId)
        {
            CustomMaterialPropValue propValue = GetPropValue(materialId, propId);

            if (propValue != null)
            {
                PropValues.Remove(propValue);
            }
        }

        public void SetCustomFileMaterialProp(int materialId, Guid propId, string filePath)
        {
            CustomMaterialPropValue propValue = GetPropValue(materialId, propId);

            if (propValue == null)
            {
                PropValues.Add(new CustomMaterialPropValue()
                {
                    MaterialId = materialId,
                    PropId = propId,
                    Value = filePath
                });
            }
            else
            {
                propValue.Value = filePath;
            }
        }

        public void RemoveCustomFileMaterialProp(int materialId, Guid propId)
        {
            CustomMaterialPropValue propValue = GetPropValue(materialId, propId);

            if (propValue != null)
            {
                PropValues.Remove(propValue);
            }
        }

        #region Private helpers

        // Returns null if no matching prop value was found.
        private CustomMaterialPropValue GetPropValue(int materialId, Guid propId)
        {
            return PropValues.FirstOrDefault(pv => pv.MaterialId == materialId && pv.PropId == propId);
        }

        #endregion
    }
}
