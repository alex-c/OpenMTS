using System;

namespace OpenMTS.Models
{
    /// <summary>
    /// Represents a custom material prop value.
    /// </summary>
    public class CustomMaterialPropValue
    {
        /// <summary>
        /// ID of the material, this prop value is for.
        /// </summary>
        public int MaterialId { get; set; }

        /// <summary>
        /// ID of the prop, this value is for.
        /// </summary>
        public Guid PropId { get; set; }

        /// <summary>
        /// Value of the prop to set.
        /// </summary>
        public object Value { get; set; }
    }
}
