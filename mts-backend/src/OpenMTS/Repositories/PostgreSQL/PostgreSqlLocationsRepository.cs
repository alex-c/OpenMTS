using Dapper;
using Microsoft.Extensions.Configuration;
using OpenMTS.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace OpenMTS.Repositories.PostgreSQL
{
    /// <summary>
    /// A PostgreSQL implementation of the plastics repository.
    /// </summary>
    /// <seealso cref="OpenMTS.Repositories.PostgreSQL.PostgreSqlRepositoryBase" />
    /// <seealso cref="OpenMTS.Repositories.ILocationsRepository" />
    public class PostgreSqlLocationsRepository : PostgreSqlRepositoryBase, ILocationsRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PostgreSqlLocationsRepository"/> class.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        public PostgreSqlLocationsRepository(IConfiguration configuration) : base(configuration) { }

        /// <summary>
        /// Gets all available storage site.
        /// </summary>
        /// <returns>
        /// Returns all storage sites.
        /// </returns>
        public IEnumerable<StorageSite> GetAllStorageSites()
        {
            Dictionary<Guid, StorageSite> siteMap = new Dictionary<Guid, StorageSite>();
            string sql = "SELECT * FROM storage_sites s LEFT JOIN storage_areas a ON a.site_id = s.id";

            IEnumerable<StorageSite> sites = null;
            using (IDbConnection connection = GetNewConnection())
            {
                sites = connection.Query<StorageSite, StorageArea, StorageSite>(sql,
                    (site, area) =>
                    {
                        StorageSite storageSite = null;
                        if (!siteMap.TryGetValue(site.Id, out storageSite))
                        {
                            storageSite = site;
                            siteMap.Add(storageSite.Id, storageSite);
                        }
                        storageSite.Areas.Add(area);
                        return storageSite;
                    }, splitOn: "id");
            }
            return sites.Distinct();
        }

        /// <summary>
        /// Searches storage sites using a partial name.
        /// </summary>
        /// <param name="partialName">String to search for in site names.</param>
        /// <returns>
        /// Returns filtered storage sites.
        /// </returns>
        public IEnumerable<StorageSite> SearchStorageSitesByName(string partialName)
        {
            string name = $"%{partialName}%";
            Dictionary<Guid, StorageSite> siteMap = new Dictionary<Guid, StorageSite>();
            string sql = "SELECT * FROM storage_sites s LEFT JOIN storage_areas a ON a.site_id = s.id WHERE s.name ILIKE @Name";

            IEnumerable<StorageSite> sites = null;
            using (IDbConnection connection = GetNewConnection())
            {
                sites = connection.Query<StorageSite, StorageArea, StorageSite>(sql, (site, area) =>
                {
                    StorageSite storageSite = null;
                    if (!siteMap.TryGetValue(site.Id, out storageSite))
                    {
                        storageSite = site;
                        siteMap.Add(storageSite.Id, storageSite);
                    }
                    storageSite.Areas.Add(area);
                    return storageSite;
                },
                param: new { name },
                splitOn: "id");
            }
            return sites.Distinct();
        }

        /// <summary>
        /// Gets a storage site by it's ID.
        /// </summary>
        /// <param name="id">ID of the storage site to get.</param>
        /// <returns>
        /// Returns the storage site or null.
        /// </returns>
        public StorageSite GetStorageSite(Guid id)
        {
            Dictionary<Guid, StorageSite> siteMap = new Dictionary<Guid, StorageSite>();
            string sql = "SELECT * FROM storage_sites s LEFT JOIN storage_areas a ON a.site_id = s.id WHERE s.id = @Id";

            IEnumerable<StorageSite> sites = null;
            using (IDbConnection connection = GetNewConnection())
            {
                sites = connection.Query<StorageSite, StorageArea, StorageSite>(sql, (site, area) =>
                {
                    StorageSite storageSite = null;
                    if (!siteMap.TryGetValue(site.Id, out storageSite))
                    {
                        storageSite = site;
                        siteMap.Add(storageSite.Id, storageSite);
                    }
                    storageSite.Areas.Add(area);
                    return storageSite;
                },
                param: new { id },
                splitOn: "id");
            }
            return sites.Distinct().FirstOrDefault();
        }

        /// <summary>
        /// Creates a new storage site.
        /// </summary>
        /// <param name="name">Name of the new storage site.</param>
        /// <returns>
        /// Returns the newly created storage site.
        /// </returns>
        public StorageSite CreateStorageSite(string name)
        {
            Guid id = Guid.NewGuid();

            StorageSite site = null;
            using (IDbConnection connection = GetNewConnection())
            {
                connection.Execute("INSERT INTO storage_sites (id, name) VALUES (@Id, @Name)", new { id, name });
                site = connection.QuerySingleOrDefault<StorageSite>("SELECT * FROM storage_sites WHERE id=@Id", new { id });
            }
            return site;
        }

        /// <summary>
        /// Creates a new area for an existing storage site.
        /// </summary>
        /// <param name="storageSite">Site to add an area to.</param>
        /// <param name="areaName">Name of the area to create.</param>
        /// <returns>
        /// Returns the newly created area.
        /// </returns>
        public StorageArea CreateStorageArea(StorageSite storageSite, string areaName)
        {
            Guid id = Guid.NewGuid();

            StorageArea area = null;
            using (IDbConnection connection = GetNewConnection())
            {
                connection.Execute("INSERT INTO storage_areas (id, name, site_id) VALUES (@Id, @Name, @SiteId)", new
                {
                    Id = id,
                    Name = areaName,
                    SiteId = storageSite.Id
                });
                area = connection.QuerySingleOrDefault<StorageArea>("SELECT * FROM storage_areas WHERE id=@Id", new { id });
            }
            return area;
        }

        /// <summary>
        /// Updates an existing storage site. This does not update associated storage areas.
        /// </summary>
        /// <param name="storageSite">Storage site to update.</param>
        /// <returns>
        /// Returns the updated storage site.
        /// </returns>
        public StorageSite UpdateStorageSite(StorageSite storageSite)
        {
            using (IDbConnection connection = GetNewConnection())
            {
                connection.Execute("UPDATE storage_sites SET name=@Name WHERE id=@Id", new { storageSite.Id, storageSite.Name });
            }
            return storageSite;
        }

        /// <summary>
        /// Updates a storage area.
        /// </summary>
        /// <param name="storageArea">Area to update.</param>
        /// <returns>
        /// Returns the updated area.
        /// </returns>
        public StorageArea UpdateStorageArea(StorageArea storageArea)
        {
            using (IDbConnection connection = GetNewConnection())
            {
                connection.Execute("UPDATE storage_areas SET name=@Name WHERE id=@Id", new { storageArea.Id, storageArea.Name });
            }
            return storageArea;
        }

        /// <summary>
        /// Delets a storage site.
        /// </summary>
        /// <param name="id">ID of the storage site to delete.</param>
        /// <exception cref="NotImplementedException"></exception>
        public void DeleteStorageSite(Guid id)
        {
            using (IDbConnection connection = GetNewConnection())
            {
                connection.Execute("DELETE FROM storage_areas WHERE site_id=@Id", new { id });
                connection.Execute("DELETE FROM storage_sites WHERE id=@Id", new { id });
            }
        }
    }
}
