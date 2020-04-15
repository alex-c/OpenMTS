using OpenMTS.Models.Environmnt;

namespace OpenMTS.Models.Trace
{
    /// <summary>
    /// The result of a material data trace request.
    /// </summary>
    public class TraceResult
    {
        /// <summary>
        /// The material batch.
        /// </summary>
        public MaterialBatch Batch { get; set; }

        /// <summary>
        /// The transaction of the original checkin of the batch.
        /// </summary>
        public Transaction CheckInTransaction { get; set; }

        /// <summary>
        /// The check-out transaction.
        /// </summary>
        public Transaction CheckOutTransaction { get; set; }

        /// <summary>
        /// The temperature extrema recorded at the batche's storage location during the storage time.
        /// </summary>
        public Extrema Temperature { get; set; }

        /// <summary>
        /// The humidity extrema recorded at the batche's storage location during the storage time.
        /// </summary>
        public Extrema Humidity { get; set; }
    }
}
