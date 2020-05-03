using Dapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using OpenMTS.Models;
using OpenMTS.Models.Stats;
using System.Collections.Generic;
using System.Data;

namespace OpenMTS.Repositories.PostgreSQL
{
    /// <summary>
    /// A PostgreSQL implementation of the stats provider.
    /// </summary>
    /// <seealso cref="OpenMTS.Repositories.PostgreSQL.PostgreSqlRepositoryBase" />
    /// <seealso cref="OpenMTS.Repositories.IStatsProvider" />
    public class PostgreSqlStatsProvider : PostgreSqlRepositoryBase, IStatsProvider
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PostgreSqlStatsProvider"/> class.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        public PostgreSqlStatsProvider(IConfiguration configuration, ILogger<PostgreSqlStatsProvider> logger) : base(configuration, logger) { }

        /// <summary>
        /// Gets an overview of all available storage sites, which includes latest environmental values and total material.
        /// </summary>
        /// <returns>
        /// Returns the overview of each storage.
        /// </returns>
        public IEnumerable<StorageSiteOverview> GetSitesOverview()
        {
            string sql = "SELECT SUM(b.quantity) AS total_material,s.id,s.name FROM storage_sites s LEFT JOIN storage_areas a ON a.site_id=s.id LEFT JOIN batches b ON b.area_id=a.id GROUP BY s.id";

            IEnumerable<StorageSiteOverview> overview = null;
            using (IDbConnection connection = GetNewConnection())
            {
                overview = connection.Query<StorageSiteOverview, StorageSite, StorageSiteOverview>(sql,
                    map: (siteOverview, site) =>
                    {
                        if (siteOverview == null)
                        {
                            siteOverview = new StorageSiteOverview();
                        }
                        siteOverview.Site = site;
                        return siteOverview;
                    },
                    splitOn: "id");
            }
            return overview;
        }
    }
}
