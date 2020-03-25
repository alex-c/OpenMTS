using Microsoft.Extensions.Logging;
using OpenMTS.Models;
using OpenMTS.Repositories;
using System;
using System.Collections.Generic;

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
        /// Gets the transaction log for a given material batch.
        /// </summary>
        /// <param name="materialBatchId">The ID of the material batch to read the log for.</param>
        /// <returns>Returns the transaction log.</returns>
        public IEnumerable<Transaction> GetTransactionLog(Guid materialBatchId)
        {
            return TransactionRepository.GetTransactionsForBatch(materialBatchId);
        }
    }
}
