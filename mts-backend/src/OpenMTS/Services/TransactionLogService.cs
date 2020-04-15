using Microsoft.Extensions.Logging;
using OpenMTS.Models;
using OpenMTS.Repositories;
using OpenMTS.Services.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OpenMTS.Services
{
    /// <summary>
    /// A service exposing material batch transaction logging functionality.
    /// </summary>
    public class TransactionLogService
    {
        /// <summary>
        /// The underlying repository providing transaction data.
        /// </summary>
        private ITransactionRepository TransactionRepository { get; }

        /// <summary>
        /// A logger for local logging needs.
        /// </summary>
        private ILogger Logger { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="TransactionLogService"/> class.
        /// </summary>
        /// <param name="loggerFactory">The factory to create loggers from.</param>
        /// <param name="transactionRepository">The repository providing transaction data.</param>
        public TransactionLogService(ILoggerFactory loggerFactory, ITransactionRepository transactionRepository)
        {
            Logger = loggerFactory.CreateLogger<TransactionLogService>();
            TransactionRepository = transactionRepository;
        }

        /// <summary>
        /// Gets the transaction log for a given material batch, ordered from newest log entry to oldest.
        /// </summary>
        /// <param name="materialBatchId">The ID of the material batch to read the log for.</param>
        /// <returns>Returns the transaction log.</returns>
        public IEnumerable<Transaction> GetTransactionLog(Guid materialBatchId)
        {
            return TransactionRepository.GetTransactionsForBatch(materialBatchId).OrderByDescending(t => t.Timestamp);
        }

        /// <summary>
        /// Gets the last transaction log entry for a given material batch.
        /// </summary>
        /// <param name="materialBatchId">The ID of the batch.</param>
        /// <returns>Returns the last transaction.</returns>
        public Transaction GetLastTransactionLogEntry(Guid materialBatchId)
        {
            return TransactionRepository.GetLastTransactionForBatch(materialBatchId);
        }

        /// <summary>
        /// Gets a transaction by it's unique ID.
        /// </summary>
        /// <param name="transactionId">The ID of the transaction to get.</param>
        /// <returns>Returns the transaction.</returns>
        /// <exception cref="TransactionNotFoundException">Thrown if no matching transaction could be found.</exception>
        public Transaction GetTransaction(Guid transactionId)
        {
            Transaction transaction = TransactionRepository.GetTransaction(transactionId);
            if (transaction == null)
            {
                throw new TransactionNotFoundException();
            }
            return transaction;
        }

        /// <summary>
        /// Logs a new  transaction.
        /// </summary>
        /// <param name="transaction">The transaction to log.</param>
        public void LogTransaction(Transaction transaction)
        {
            TransactionRepository.LogTransaction(transaction);
        }

        /// <summary>
        /// Attempts to amend the last transaction log entry of a given batch.
        /// </summary>
        /// <param name="materialBatchId">The ID of the batch.</param>
        /// <param name="transactionId">The ID of the transaction to amend.</param>
        /// <param name="quantity">The quantity to set.</param>
        /// <exception cref="OpenMTS.Services.Exceptions.NotLastLogEntryException">Thrown if the passed transaction ID doesn't match the last transaction of this batch.</exception>
        public void AmendLastTransactionLogEntry(Guid materialBatchId, Guid transactionId, double quantity)
        {
            Transaction transaction = GetLastTransactionLogEntry(materialBatchId);
            if (transaction.Id != transactionId)
            {
                throw new NotLastLogEntryException(transactionId);
            }
            transaction.Quantity = quantity;
            TransactionRepository.UpdateTransaction(transaction);
        }
    }
}
