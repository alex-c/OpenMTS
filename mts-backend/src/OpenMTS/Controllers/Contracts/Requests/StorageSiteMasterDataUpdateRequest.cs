namespace OpenMTS.Controllers.Contracts.Requests
{
    /// <summary>
    /// A contract for requests to update a storage site's master data.
    /// </summary>
    public class StorageSiteMasterDataUpdateRequest
    {
        /// <summary>
        /// The name to update.
        /// </summary>
        public string Name { get; set; }
    }
}
