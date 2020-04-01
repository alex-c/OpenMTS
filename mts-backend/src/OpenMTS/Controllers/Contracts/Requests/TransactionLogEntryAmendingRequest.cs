namespace OpenMTS.Controllers.Contracts.Requests
{
    /// <summary>
    /// A contract for a request to amend a material batch transaction log entry.
    /// </summary>
    public class TransactionLogEntryAmendingRequest
    {
        /// <summary>
        /// The new quantity of the log entry. Is negative for check-out transactions.
        /// </summary>
        public double Quantity { get; set; }
    }
}
