using System;

namespace OpenMTS.Services.Exceptions
{
    /// <summary>
    /// Indicates that a material type ID is already taken.
    /// </summary>
    /// <seealso cref="System.Exception" />
    /// <seealso cref="OpenMTS.Services.Exceptions.IResourceAlreadyExsistsException" />
    public class MaterialTypeAlreadyExistsException : Exception, IResourceAlreadyExsistsException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MaterialTypeAlreadyExistsException"/> class.
        /// </summary>
        public MaterialTypeAlreadyExistsException() : base("This material type ID is already taken.") { }

        /// <summary>
        /// Initializes a new instance of the <see cref="MaterialTypeAlreadyExistsException"/> class.
        /// </summary>
        /// <param name="id">The ID that is already taken.</param>
        public MaterialTypeAlreadyExistsException(string id) : base($"The material type ID `{id}` is already taken.") { }
    }
}
