using Dapper;
using Microsoft.Extensions.Configuration;
using OpenMTS.Models;
using OpenMTS.Models.Environmnt;
using System;
using System.Collections.Generic;
using System.Data;

namespace OpenMTS.Repositories.PostgreSQL
{
    /// <summary>
    /// PostgreSQL implementation of the environmental data repository.
    /// </summary>
    /// <seealso cref="OpenMTS.Repositories.PostgreSQL.PostgreSqlRepositoryBase" />
    /// <seealso cref="OpenMTS.Repositories.IEnvironmentalDataRepository" />
    public class PostgreSqlEnvironmentalDataRepository : PostgreSqlRepositoryBase, IEnvironmentalDataRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PostgreSqlEnvironmentalDataRepository"/> class.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        public PostgreSqlEnvironmentalDataRepository(IConfiguration configuration) : base(configuration) { }

        /// <summary>
        /// Gets the latest value for a specific storage site and environmental factor.
        /// </summary>
        /// <param name="site">The storage site to get the latest value for.</param>
        /// <param name="factor">The factor to get the latest value for.</param>
        /// <returns>
        /// Returns the latest value, or null.
        /// </returns>
        public DataPoint GetLatestValue(StorageSite site, EnvironmentalFactor factor)
        {
            string column = GetColumnName(factor);

            DataPoint value = null;
            using (IDbConnection connection = GetNewConnection())
            {
                value = connection.QuerySingleOrDefault<DataPoint>($"SELECT timestamp,{column} AS value FROM environment WHERE site=@Id AND timestamp>now()-interval '1day' ORDER BY timestamp DESC LIMIT 1", new
                {
                    site.Id
                });
            }
            return value;
        }

        /// <summary>
        /// Gets the history of a specific storage site and environmental factor.
        /// </summary>
        /// <param name="site">The storage site to get the history for.</param>
        /// <param name="factor">The factor to get the history for.</param>
        /// <param name="startTime">Start time of the period to get value for.</param>
        /// <param name="endTime">End time of the period to get value for.</param>
        /// <returns>
        /// Returns the matching values.
        /// </returns>
        public IEnumerable<DataPoint> GetHistory(StorageSite site, EnvironmentalFactor factor, DateTime startTime, DateTime endTime)
        {
            string column = GetColumnName(factor);

            IEnumerable<DataPoint> history = null;
            using (IDbConnection connection = GetNewConnection())
            {
                history = connection.Query<DataPoint>($"SELECT timestamp,{column} AS value FROM environment WHERE site=@Id AND timestamp>@StartTime AND timestamp<@EndTime ORDER BY timestamp ASC", new
                {
                    site.Id,
                    startTime,
                    endTime
                });
            }
            return history;
        }

        /// <summary>
        /// Gets the min and max values for a specific storage site and environmental factor.
        /// </summary>
        /// <param name="site">The storage site to get the history for.</param>
        /// <param name="factor">The factor to get the history for.</param>
        /// <param name="startTime">Start time of the period to get value for.</param>
        /// <param name="endTime">End time of the period to get value for.</param>
        /// <returns>
        /// Returns the extrema.
        /// </returns>
        public Extrema GetExtrema(StorageSite site, EnvironmentalFactor factor, DateTime startTime, DateTime endTime)
        {
            string column = GetColumnName(factor);

            Extrema extrema = null;
            using (IDbConnection connection = GetNewConnection())
            {
                extrema = connection.QuerySingleOrDefault<Extrema>($"SELECT min({column}) AS min_value, max({column}) AS max_value FROM environment WHERE site=@Id AND timestamp>@StartTime AND timestamp<@EndTime", new
                {
                    site.Id,
                    startTime,
                    endTime
                });
            }
            return extrema;
        }

        /// <summary>
        /// Records an environmental snapshot.
        /// </summary>
        /// <param name="snapshot">The snapshot to record.</param>
        public void RecordEnvironmentalSnapshot(EnvironmentSnapshot snapshot)
        {
            using (IDbConnection connection = GetNewConnection())
            {
                connection.Execute("INSERT INTO environment (timestamp, site, temperature, humidity) VALUES (@Timestamp, @StorageSiteId, @Temperature, @Humidity)", snapshot);
            }
        }

        /// <summary>
        /// Gets the database colum name for an environmental factor..
        /// </summary>
        /// <param name="factor">The factor to get the column name for.</param>
        /// <returns>Returns the column name</returns>
        /// <exception cref="NotImplementedException"/>
        private string GetColumnName(EnvironmentalFactor factor)
        {
            switch (factor)
            {
                case EnvironmentalFactor.Temperature:
                    return "temperature";
                case EnvironmentalFactor.Humidity:
                    return "humidity";
                default: throw new NotImplementedException();
            }
        }
    }
}
