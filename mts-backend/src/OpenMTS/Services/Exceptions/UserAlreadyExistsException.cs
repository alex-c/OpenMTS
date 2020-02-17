using System;

namespace OpenMTS.Services.Exceptions
{
    /// <summary>
    /// Indicates that an user already exsists.
    /// </summary>
    public class UserAlreadyExistsException : Exception, IResourceAlreadyExsistsException
    {
        /// <summary>
        /// Creates a generic user-already-exists exception.
        /// </summary>
        public UserAlreadyExistsException() : base("A user with that ID already exists.") { }

        /// <summary>
        /// Indicates the user ID which already exists.
        /// </summary>
        /// <param name="id">User ID that already exists.</param>
        public UserAlreadyExistsException(string id) : base($"A user with ID `{id}` already exists.") { }
    }
}
