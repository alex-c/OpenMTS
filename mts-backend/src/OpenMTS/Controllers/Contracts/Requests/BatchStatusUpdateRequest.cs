namespace OpenMTS.Controllers.Contracts.Requests
{
    /// <summary>
    /// A contract for a request to update a material batche's status (whether it is locked or not).
    /// </summary>
    public class BatchStatusUpdateRequest
    {
        /// <summary>
        /// Whether the batch should be locked.
        /// </summary>
        public bool IsLocked { get; set; }
    }
}
