using System;

namespace OpenMTS.Services.Exceptions
{
    /// <summary>
    /// Indicates that a custom property could not be found.
    /// </summary>
    /// <seealso cref="Exception" />
    /// <seealso cref="IResourceNotFoundException" />
    public class CustomPropNotFoundException : Exception, IResourceNotFoundException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CustomPropNotFoundException"/> class.
        /// </summary>
        public CustomPropNotFoundException() : base("Custom property could not be found.") { }

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomPropNotFoundException"/> class.
        /// </summary>
        /// <param name="id">The ID of the prop that could not be found.</param>
        public CustomPropNotFoundException(Guid id) : base($"Custom property with ID `{id}` could not be found.") { }
    }
}
