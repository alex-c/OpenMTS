using Dapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using OpenMTS.Models;
using System;
using System.Collections.Generic;
using System.Data;

namespace OpenMTS.Repositories.PostgreSQL
{
    /// <summary>
    /// A PostgreSQL implementation of the custom material prop value repository.
    /// </summary>
    /// <seealso cref="OpenMTS.Repositories.PostgreSQL.PostgreSqlRepositoryBase" />
    /// <seealso cref="OpenMTS.Repositories.ICustomMaterialPropValueRepository" />
    public class PostgreSqlCustomMaterialPropValueRepository : PostgreSqlRepositoryBase, ICustomMaterialPropValueRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PostgreSqlCustomMaterialPropValueRepository"/> class.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        public PostgreSqlCustomMaterialPropValueRepository(IConfiguration configuration, ILogger<PostgreSqlCustomMaterialPropValueRepository> logger) : base(configuration, logger) { }

        /// <summary>
        /// Sets a custom material property of the text type.
        /// </summary>
        /// <param name="materialId">The ID of the material to set the prop value for.</param>
        /// <param name="propId">The ID of the prop to set.</param>
        /// <param name="text">The text value to set.</param>
        /// <exception cref="NotImplementedException"></exception>
        public void SetCustomTextMaterialProp(int materialId, Guid propId, string text)
        {
            using (IDbConnection connection = GetNewConnection())
            {
                connection.Execute("INSERT INTO text_material_prop_values (material_id, prop_id, value) VALUES (@MaterialId, @PropId, @Text) ON CONFLICT (material_id,prop_id) DO UPDATE SET value=@Text",
                    param: new
                    {
                        materialId,
                        propId,
                        text
                    });
            }
        }

        /// <summary>
        /// Sets a custom file material prop of the file type. This just persists the file path of a file already persisted to the FS by the
        /// <see cref="OpenMTS.Services.MaterialsService" />.
        /// </summary>
        /// <param name="materialId">The ID of the material to set the prop value for.</param>
        /// <param name="propId">The ID of the prop to set.</param>
        /// <param name="filePath">The file path to set.</param>
        /// <exception cref="NotImplementedException"></exception>
        public void SetCustomFileMaterialProp(int materialId, Guid propId, string filePath)
        {
            using (IDbConnection connection = GetNewConnection())
            {
                connection.Execute("INSERT INTO file_material_prop_values (material_id, prop_id, file_path) VALUES (@MaterialId, @PropId, @FilePath) ON CONFLICT (material_id,prop_id) DO UPDATE SET file_path=@FilePath",
                    param: new
                    {
                        materialId,
                        propId,
                        filePath
                    });
            }
        }

        /// <summary>
        /// Deletes a custom material property of the text type.
        /// </summary>
        /// <param name="materialId">The ID of the material to delete the prop value for.</param>
        /// <param name="propId">The ID of the prop to unset.</param>
        /// <exception cref="NotImplementedException"></exception>
        public void RemoveCustomTextMaterialProp(int materialId, Guid propId)
        {
            DeleteCustomMaterialProp(materialId, propId, PropType.Text);
        }

        /// <summary>
        /// Deletes a custom material property of the file type.
        /// </summary>
        /// <param name="materialId">The ID of the material to delete the prop value for.</param>
        /// <param name="propId">The ID of the prop to unset.</param>
        /// <exception cref="NotImplementedException"></exception>
        public void RemoveCustomFileMaterialProp(int materialId, Guid propId)
        {
            DeleteCustomMaterialProp(materialId, propId, PropType.File);
        }

        #region Private helpers

        /// <summary>
        /// Deletes the custom material property.
        /// </summary>
        /// <param name="materialId">The ID of the material to delete the prop value for.</param>
        /// <param name="propId">The ID of the prop to unset.</param>
        /// <param name="type">The type of the prop to unset.</param>
        private void DeleteCustomMaterialProp(int materialId, Guid propId, PropType type)
        {
            string table = type == PropType.Text ? "text_material_prop_values" : "file_material_prop_values";
            string sql = $"DELETE FROM {table} WHERE material_id=@MaterialId AND prop_id=@PropId";
            using (IDbConnection connection = GetNewConnection())
            {
                connection.Execute(sql, new { materialId, propId });
            }
        }

        #endregion
    }
}
