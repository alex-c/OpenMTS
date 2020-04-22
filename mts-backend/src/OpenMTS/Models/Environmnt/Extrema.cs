namespace OpenMTS.Models.Environmnt
{
    /// <summary>
    /// Represents the min and max value recorded for a specific environmental factor at a specific storage site during a specific time period.
    /// </summary>
    public class Extrema
    {
        /// <summary>
        /// Gets minimum recorded value.
        /// </summary>
        public float? MinValue { get; set; }

        /// <summary>
        /// Gets maximum recorded value.
        /// </summary>
        public float? MaxValue { get; set; }
    }
}
