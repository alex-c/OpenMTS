using System;

namespace OpenMTS.Services.Exceptions
{
    /// <summary>
    /// Indicates a material could not be found.
    /// </summary>
    /// <seealso cref="System.Exception" />
    /// <seealso cref="OpenMTS.Services.Exceptions.IResourceNotFoundException" />
    public class MaterialNotFoundException : Exception, IResourceNotFoundException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MaterialNotFoundException"/> class.
        /// </summary>
        public MaterialNotFoundException() : base("The requested material could not be found.") { }

        /// <summary>
        /// Initializes a new instance of the <see cref="MaterialNotFoundException"/> class.
        /// </summary>
        /// <param name="id">The ID of the material that could not be found.</param>
        public MaterialNotFoundException(int id) : base($"The material with ID `{id}` could not be found.") { }
    }
}
