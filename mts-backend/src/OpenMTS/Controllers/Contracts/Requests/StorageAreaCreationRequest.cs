namespace OpenMTS.Controllers.Contracts.Requests
{
    /// <summary>
    /// A request contract for the creation of storage areas.
    /// </summary>
    public class StorageAreaCreationRequest
    {
        /// <summary>
        /// The name of the storage area to create.
        /// </summary>
        public string Name { get; set; }
    }
}
