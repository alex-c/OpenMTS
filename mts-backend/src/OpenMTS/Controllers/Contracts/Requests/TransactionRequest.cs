namespace OpenMTS.Controllers.Contracts.Requests
{
    /// <summary>
    /// A request to perform a transaction: checking material in or out of inventory.
    /// </summary>
    public class TransactionRequest
    {
        /// <summary>
        /// Whether the transaction is a check-out.
        /// </summary>
        public bool IsCheckout { get; set; }

        /// <summary>
        /// Quantity of the transaction.
        /// </summary>
        public double Quantity { get; set; }
    }
}
