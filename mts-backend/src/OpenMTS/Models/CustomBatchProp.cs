using System;

namespace OpenMTS.Models
{
    /// <summary>
    /// Represents a cusom material batch property.
    /// </summary>
    public class CustomBatchProp
    {
        /// <summary>
        /// ID of the custom prop.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Name of the custom prop.
        /// </summary>
        public string Name { get; set; }
    }
}
