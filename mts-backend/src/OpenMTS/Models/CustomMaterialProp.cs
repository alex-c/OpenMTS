using System;

namespace OpenMTS.Models
{
    /// <summary>
    /// Defines a custom property for materials.
    /// </summary>
    public class CustomMaterialProp
    {
        /// <summary>
        /// The ID of the property.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// The name of the property.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The type of the property.
        /// </summary>
        public PropType Type { get; set; }
    }
}
