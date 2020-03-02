namespace OpenMTS.Models
{
    /// <summary>
    /// Represent an access right.
    /// </summary>
    public class Right
    {
        /// <summary>
        /// The ID of this right.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Initializes this right instance.
        /// </summary>
        /// <param name="id">ID of the access right.</param>
        public Right(string id)
        {
            Id = id;
        }
    }
}
