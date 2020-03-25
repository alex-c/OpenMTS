namespace OpenMTS.Controllers.Contracts.Requests
{
    /// <summary>
    /// A contract for a request to set a custom material prop of the text type.
    /// </summary>
    public class SetCustomTextMaterialPropRequest
    {
        /// <summary>
        /// The text to set.
        /// </summary>
        public string Text { get; set; }
    }
}
