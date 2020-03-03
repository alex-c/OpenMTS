namespace OpenMTS.Controllers.Contracts.Requests
{
    /// <summary>
    /// A request contract for updating storage areas.
    /// </summary>
    public class StorageAreaUpdateRequest
    {
        /// <summary>
        /// The name of the storage area to update.
        /// </summary>
        public string Name { get; set; }
    }
}
