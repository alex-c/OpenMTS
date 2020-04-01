using OpenMTS.Models;
using System;
using System.Collections.Generic;

namespace OpenMTS.Repositories
{
    /// <summary>
    /// A generic interface for a repository of logged transaction.
    /// </summary>
    public interface ITransactionRepository
    {
        /// <summary>
        /// Gets the transaction log for a given material batch.
        /// </summary>
        /// <param name="materialBatchId">The ID of the batch.</param>
        /// <returns>Returns all matching transactions</returns>
        IEnumerable<Transaction> GetTransactionsForBatch(Guid materialBatchId);

        /// <summary>
        /// Gets the last transaction from a material batch transaction log.
        /// </summary>
        /// <param name="materialBatchId">The ID of the batch.</param>
        /// <returns>Returns the last transaction.</returns>
        Transaction GetLastTransactionForBatch(Guid materialBatchId);

        /// <summary>
        /// Logs a new transaction.
        /// </summary>
        /// <param name="transaction">The transaction to log.</param>
        void LogTransaction(Transaction transaction);

        /// <summary>
        /// Updates a transaction.
        /// </summary>
        /// <param name="transaction">The transaction to update.</param>
        void UpdateTransaction(Transaction transaction);
    }
}
