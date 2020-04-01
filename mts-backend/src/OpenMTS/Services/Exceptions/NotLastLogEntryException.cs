using System;

namespace OpenMTS.Services.Exceptions
{
    /// <summary>
    /// Indicates that a log entry that was expected to be the last log entry of a material batch transaction log in fact isn't.
    /// </summary>
    /// <seealso cref="System.Exception" />
    public class NotLastLogEntryException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NotLastLogEntryException"/> class.
        /// </summary>
        public NotLastLogEntryException() : base("The log entry is not the last log entry.") { }

        /// <summary>
        /// Initializes a new instance of the <see cref="NotLastLogEntryException"/> class.
        /// </summary>
        /// <param name="id">The ID of the log entry that isn't the last entry of it's log.</param>
        public NotLastLogEntryException(Guid id) : base($"The log entry with ID `{id}` is not the last log entry.") { }
    }
}
