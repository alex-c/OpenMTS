using System;

namespace OpenMTS.Services.Exceptions
{
    /// <summary>
    /// Indicates that a material type could not be found.
    /// </summary>
    /// <seealso cref="System.Exception" />
    /// <seealso cref="OpenMTS.Services.Exceptions.IResourceNotFoundException" />
    public class MaterialTypeNotFoundException : Exception, IResourceNotFoundException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MaterialTypeNotFoundException"/> class.
        /// </summary>
        public MaterialTypeNotFoundException() : base("No matching material type found.") { }

        /// <summary>
        /// Initializes a new instance of the <see cref="MaterialTypeNotFoundException"/> class.
        /// </summary>
        /// <param name="id">The ID that oculd not be found.</param>
        public MaterialTypeNotFoundException(string id) : base($"No material type with ID `{id}` found.") { }
    }
}
