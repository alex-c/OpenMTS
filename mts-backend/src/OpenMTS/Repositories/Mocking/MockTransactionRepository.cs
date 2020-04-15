using OpenMTS.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OpenMTS.Repositories.Mocking
{
    public class MockTransactionRepository : ITransactionRepository
    {
        private Dictionary<Guid, List<Transaction>> BatchTransactions { get; }

        public MockTransactionRepository(MockDataProvider mockDataProvider = null)
        {
            if (mockDataProvider == null)
            {
                BatchTransactions = new Dictionary<Guid, List<Transaction>>();
            }
            else
            {
                BatchTransactions = mockDataProvider.BatchTransactions;
            }
        }

        public IEnumerable<Transaction> GetTransactionsForBatch(Guid materialBatchId)
        {
            return BatchTransactions.GetValueOrDefault(materialBatchId);
        }

        public Transaction GetLastTransactionForBatch(Guid materialBatchId)
        {
            return BatchTransactions.GetValueOrDefault(materialBatchId).OrderByDescending(t => t.Timestamp).FirstOrDefault();
        }

        public Transaction GetTransaction(Guid transactionId)
        {
            List<Transaction> transactions = BatchTransactions.Values.Aggregate((result, list) => { result.AddRange(list); return result; });
            return transactions.FirstOrDefault();
        }

        public void LogTransaction(Transaction transaction)
        {
            if (!BatchTransactions.ContainsKey(transaction.MaterialBatchId))
            {
                BatchTransactions.Add(transaction.MaterialBatchId, new List<Transaction>() { transaction });
            }
            else
            {
                BatchTransactions[transaction.MaterialBatchId].Add(transaction);
            }
        }

        public void UpdateTransaction(Transaction transaction)
        {
            // No need to do anything...
        }
    }
}
