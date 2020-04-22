namespace OpenMTS.Models.Stats
{
    /// <summary>
    /// An overview over a storage site, including total material and latest environmental values.
    /// </summary>
    public class StorageSiteOverview
    {
        /// <summary>
        /// The site this overview is for.
        /// </summary>
        public StorageSite Site { get; set; }

        /// <summary>
        /// The total quantity of material stored at this site.
        /// </summary>
        public double TotalMaterial { get; set; }

        /// <summary>
        /// The latest temperature recorded at this site.
        /// </summary>
        public float? Temperature { get; set; }

        /// <summary>
        /// The latest humidity recorded at this site.
        /// </summary>
        public float? Humidity { get; set; }
    }
}
