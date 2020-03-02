namespace OpenMTS.Controllers.Contracts.Requests
{
    /// <summary>
    /// A request contract for the creation of storage sites.
    /// </summary>
    public class StorageSiteCreationRequest
    {
        /// <summary>
        /// The name of the storage site to create.
        /// </summary>
        public string Name { get; set; }
    }
}
