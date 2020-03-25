using System;

namespace OpenMTS.Services.Exceptions
{
    /// <summary>
    /// Indicates that a plastic could not be found.
    /// </summary>
    /// <seealso cref="System.Exception" />
    /// <seealso cref="OpenMTS.Services.Exceptions.IResourceNotFoundException" />
    public class PlasticNotFoundException : Exception, IResourceNotFoundException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PlasticNotFoundException"/> class.
        /// </summary>
        public PlasticNotFoundException() : base("No matching plastic found.") { }

        /// <summary>
        /// Initializes a new instance of the <see cref="PlasticNotFoundException"/> class.
        /// </summary>
        /// <param name="id">The ID that could not be found.</param>
        public PlasticNotFoundException(string id) : base($"No plasctic with ID `{id}` found.") { }
    }
}
