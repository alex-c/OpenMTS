namespace OpenMTS.Controllers.Contracts.Requests
{
    /// <summary>
    /// A contract for a request to update a material type.
    /// </summary>
    public class UpdateMaterialTypeRequest
    {
        /// <summary>
        /// The new name to set.
        /// </summary>
        public string Name { get; set; }
    }
}
