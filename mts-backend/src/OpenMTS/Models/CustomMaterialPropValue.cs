using System;

namespace OpenMTS.Models
{
    // TODO: finalize and comment
    public class CustomMaterialPropValue
    {
        public int MaterialId { get; set; }

        public Guid PropId { get; set; }

        public object Value { get; set; }
    }
}
