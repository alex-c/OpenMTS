using System;

namespace OpenMTS.Services.Exceptions
{
    /// <summary>
    /// Thrown when an transaction was not found.
    /// </summary>
    public class TransactionNotFoundException : Exception, IResourceNotFoundException
    {
        /// <summary>
        /// Creates a generic transaction-not-found exception.
        /// </summary>
        public TransactionNotFoundException() : base("Transaction could not be found.") { }

        /// <summary>
        /// Creates an exception telling what transaction ID was not found.
        /// </summary>
        /// <param name="transactionId">ID of the transaction that was not found.</param>
        public TransactionNotFoundException(Guid transactionId) : base($"Transaction `{transactionId}` could not be found.") { }
    }
}
