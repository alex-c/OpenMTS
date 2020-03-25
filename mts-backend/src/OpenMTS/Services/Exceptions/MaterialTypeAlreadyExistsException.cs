using System;

namespace OpenMTS.Services.Exceptions
{
    /// <summary>
    /// Indicates that a plastic ID is already taken.
    /// </summary>
    /// <seealso cref="System.Exception" />
    /// <seealso cref="OpenMTS.Services.Exceptions.IResourceAlreadyExsistsException" />
    public class PlasticAlreadyExistsException : Exception, IResourceAlreadyExsistsException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PlasticAlreadyExistsException"/> class.
        /// </summary>
        public PlasticAlreadyExistsException() : base("This plastic ID is already taken.") { }

        /// <summary>
        /// Initializes a new instance of the <see cref="PlasticAlreadyExistsException"/> class.
        /// </summary>
        /// <param name="id">The ID that is already taken.</param>
        public PlasticAlreadyExistsException(string id) : base($"The plastic ID `{id}` is already taken.") { }
    }
}
