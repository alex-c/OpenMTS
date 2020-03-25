using OpenMTS.Models;
using System;
using System.Collections.Generic;

namespace OpenMTS.Repositories.Mocking
{
    public class MockTransactionRepository : ITransactionRepository
    {
        private Dictionary<Guid, IEnumerable<Transaction>> BatchTransactions { get; }

        public MockTransactionRepository(MockDataProvider mockDataProvider = null)
        {
            if (mockDataProvider == null)
            {
                BatchTransactions = new Dictionary<Guid, IEnumerable<Transaction>>();
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
    }
}
