namespace OpenMTS.Models
{
    /// <summary>
    /// Represents a type of material.
    /// </summary>
    public class MaterialType
    {
        /// <summary>
        /// The ID of the material type, which is an abbreviation.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// The full name of the material type.
        /// </summary>
        public string Name { get; set; }
    }
}
