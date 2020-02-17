using System;

namespace OpenMTS.Services.Exceptions
{
    /// <summary>
    /// Thrown when a user was not found.
    /// </summary>
    public class UserNotFoundException : Exception, IResourceNotFoundException
    {
        /// <summary>
        /// Creates a generic user-not-found exception.
        /// </summary>
        public UserNotFoundException() : base("User could not be found.") { }

        /// <summary>
        /// Creates an exception what user ID was not found.
        /// </summary>
        /// <param name="userId">User ID that was not found.</param>
        public UserNotFoundException(string userId) : base($"User `{userId}` could not be found.") { }
    }
}
