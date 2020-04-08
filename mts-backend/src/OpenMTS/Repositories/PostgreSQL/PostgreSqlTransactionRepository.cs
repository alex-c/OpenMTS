using Dapper;
using Microsoft.Extensions.Configuration;
using OpenMTS.Models;
using System;
using System.Collections.Generic;
using System.Data;

namespace OpenMTS.Repositories.PostgreSQL
{
    /// <summary>
    /// A PostgreSQL implementation of the transactions repository.
    /// </summary>
    /// <seealso cref="OpenMTS.Repositories.PostgreSQL.PostgreSqlRepositoryBase" />
    /// <seealso cref="OpenMTS.Repositories.ITransactionRepository" />
    public class PostgreSqlTransactionRepository : PostgreSqlRepositoryBase, ITransactionRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PostgreSqlTransactionRepository"/> class.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        public PostgreSqlTransactionRepository(IConfiguration configuration) : base(configuration) { }

        /// <summary>
        /// Gets the transaction log for a given material batch.
        /// </summary>
        /// <param name="materialBatchId">The ID of the batch.</param>
        /// <returns>
        /// Returns all matching transactions
        /// </returns>
        public IEnumerable<Transaction> GetTransactionsForBatch(Guid materialBatchId)
        {
            IEnumerable<Transaction> transactions = null;
            using (IDbConnection connection = GetNewConnection())
            {
                transactions = connection.Query<Transaction>("SELECT batch_id AS material_batch_id,* FROM transactions WHERE batch_id=@BatchId", new
                {
                    BatchId = materialBatchId
                });
            }
            return transactions;
        }

        /// <summary>
        /// Gets the last transaction from a material batch transaction log.
        /// </summary>
        /// <param name="materialBatchId">The ID of the batch.</param>
        /// <returns>
        /// Returns the last transaction.
        /// </returns>
        public Transaction GetLastTransactionForBatch(Guid materialBatchId)
        {
            Transaction transaction = null;
            using (IDbConnection connection = GetNewConnection())
            {
                transaction = connection.QuerySingle<Transaction>("SELECT batch_id AS material_batch_id,* FROM transactions WHERE batch_id=@BatchId ORDER BY timestamp DESC LIMIT 1", new
                {
                    BatchId = materialBatchId
                });
            }
            return transaction;
        }

        /// <summary>
        /// Logs a new transaction.
        /// </summary>
        /// <param name="transaction">The transaction to log.</param>
        public void LogTransaction(Transaction transaction)
        {
            using (IDbConnection connection = GetNewConnection())
            {
                connection.Execute("INSERT INTO transactions (id,batch_id,quantity,timestamp,user_id) VALUES (@Id,@BatchId,@Quantity,@Timestamp,@UserId)", new
                {
                    transaction.Id,
                    BatchId = transaction.MaterialBatchId,
                    transaction.Quantity,
                    transaction.Timestamp,
                    transaction.UserId
                });
            }
        }

        /// <summary>
        /// Updates a transaction.
        /// </summary>
        /// <param name="transaction">The transaction to update.</param>
        public void UpdateTransaction(Transaction transaction)
        {
            using (IDbConnection connection = GetNewConnection())
            {
                connection.Execute("UPDATE transactions SET batch_id=@BatchId,quantity=@Quantity,timestamp=@Timestamp,user_id=@UserId WHERE id=@Id", new
                {
                    transaction.Id,
                    BatchId = transaction.MaterialBatchId,
                    transaction.Quantity,
                    transaction.Timestamp,
                    transaction.UserId
                });
            }
        }
    }
}
