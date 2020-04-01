using System;

namespace OpenMTS.Services.Exceptions
{
    /// <summary>
    /// Indicates that a material batch could not be found.
    /// </summary>
    /// <seealso cref="System.Exception" />
    public class MaterialBatchNotFoundException : Exception, IResourceNotFoundException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MaterialBatchNotFoundException"/> class.
        /// </summary>
        public MaterialBatchNotFoundException() : base("The material batch could not be found.") { }

        /// <summary>
        /// Initializes a new instance of the <see cref="MaterialBatchNotFoundException"/> class.
        /// </summary>
        /// <param name="id">The ID that could not be found.</param>
        public MaterialBatchNotFoundException(Guid id) : base($"The material batch with ID `{id}` could not be found.") { }
    }
}
