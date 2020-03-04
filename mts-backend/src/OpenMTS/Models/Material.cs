using System;
using System.Collections.Generic;

namespace OpenMTS.Models
{
    /// <summary>
    /// Represents a material (Werkstoff).
    /// </summary>
    public class Material
    {
        /// <summary>
        /// The unique OpenMTS ID of the material.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// A material name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// A manufacturer name.
        /// </summary>
        public string Manufacturer { get; set; }

        /// <summary>
        /// An identifier which identifies the material in the manufacturer's catalog.
        /// </summary>
        public string ManufacturerSpecificId { get; set; }

        /// <summary>
        /// The type of the material.
        /// </summary>
        public MaterialType Type { get; set; }

        /// <summary>
        /// Custom material properties.
        /// </summary>
        public ICollection<CustomMaterialPropValue> CustomProps { get; set; }
    }
}
