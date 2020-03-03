using System;

namespace OpenMTS.Models
{
    /// <summary>
    /// A storage area.
    /// </summary>
    public class StorageArea
    {
        /// <summary>
        /// Id of the storage area.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Name of the storage area.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Creates a new area with a randomly generated Guid.
        /// </summary>
        /// <param name="name">Name of the area to create.</param>
        public StorageArea(string name)
        {
            Id = Guid.NewGuid();
            Name = name;
        }
    }
}
