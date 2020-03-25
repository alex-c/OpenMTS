using System.Collections.Generic;

namespace OpenMTS.Models
{
    /// <summary>
    /// An enumeration of all access rights known to the OpenMTS system. This is currently the single source
    /// of truth for existing access rights.
    /// </summary>
    public static class RightIds
    {
        /// <summary>
        /// Gets all access right IDs.
        /// </summary>
        /// <returns>Returns the ID as a list of string.</returns>
        public static IEnumerable<string> GetAll()
        {
            return new List<string>()
            {
                // Materials
                MATERIALS_CREATE,
                MATERIALS_UPDATE,
                MATERIAL_CUSTOM_PROPS_SET,
                MATERIAL_CUSTOM_PROPS_DELETE,

                // TODO: add rights for material types

                // OpenMTS configuration
                CONFIGFURATION_SET,
                CUSTOM_MATERIAL_PROPS_CREATE,
                CUSTOM_MATERIAL_PROPS_UPDATE,
                CUSTOM_MATERIAL_PROPS_DELETE,

                // User administration
                USERS_CREATE,
                USERS_UPDATE,
                USERS_UPDATE_STATUS,

                // API key management
                KEYS_CREATE,
                KEYS_UPDATE,
                KEYS_UPDATE_STATUS,
                KEYS_DELETE,

                // Storage locations management
                STORAGE_SITES_CREATE,
                STORAGE_SITES_UPDATE,
                STORAGE_AREAS_CREATE,
                STORAGE_AREAS_UPDATE
            };
        }

        #region Materials

        /// <summary>
        /// The right that allows to create a new material.
        /// </summary>
        public static readonly string MATERIALS_CREATE = "materials.create";

        /// <summary>
        /// The right that allows to update a new material.
        /// </summary>
        public static readonly string MATERIALS_UPDATE = "materials.update";

        /// <summary>
        /// The right that allows to set a custom material prop value.
        /// </summary>
        public static readonly string MATERIAL_CUSTOM_PROPS_SET = "materials.custom_props.set";

        /// <summary>
        /// The right that allows to delete a custom material prop value.
        /// </summary>
        public static readonly string MATERIAL_CUSTOM_PROPS_DELETE = "materials.custom_props.delete";

        #endregion

        #region OpenMTS configuration

        /// <summary>
        /// The right that allows to set the OpenMTS configuration.
        /// </summary>
        public static readonly string CONFIGFURATION_SET = "configuration.set";

        /// <summary>
        /// The right that allows to create custom material props.
        /// </summary>
        public static readonly string CUSTOM_MATERIAL_PROPS_CREATE = "custom_material_props.create";

        /// <summary>
        /// The right that allows to update custom material props.
        /// </summary>
        public static readonly string CUSTOM_MATERIAL_PROPS_UPDATE = "custom_material_props.update";

        /// <summary>
        /// The right that allows to delete custom material props.
        /// </summary>
        public static readonly string CUSTOM_MATERIAL_PROPS_DELETE = "custom_material_props.delete";

        #endregion

        #region User administration

        /// <summary>
        /// The right that allows to create a new user.
        /// </summary>
        public static readonly string USERS_CREATE = "users.create";

        /// <summary>
        /// The right that allows to update an user.
        /// </summary>
        public static readonly string USERS_UPDATE = "users.update";

        /// <summary>
        /// The right that allows to update an user's status.
        /// </summary>
        public static readonly string USERS_UPDATE_STATUS = "users.update_status";

        #endregion

        #region API key administration

        /// <summary>
        /// The right that allows to query existing API keys.
        /// </summary>
        public static readonly string KEYS_QUERY = "keys.query";

        /// <summary>
        /// The right that allows to create a new API key.
        /// </summary>
        public static readonly string KEYS_CREATE = "keys.create";

        /// <summary>
        /// The right that allows to update an API key.
        /// </summary>
        public static readonly string KEYS_UPDATE = "keys.update";

        /// <summary>
        /// The right that allows to update an API key's status.
        /// </summary>
        public static readonly string KEYS_UPDATE_STATUS = "keys.update_status";

        /// <summary>
        /// The right that allows to delete an API key.
        /// </summary>
        public static readonly string KEYS_DELETE = "keys.delete";

        #endregion

        #region Locations administration

        /// <summary>
        /// The right that allows to create a new storage site.
        /// </summary>
        public static readonly string STORAGE_SITES_CREATE = "storage_sites.create";

        /// <summary>
        /// The right that allows to update a storage site.
        /// </summary>
        public static readonly string STORAGE_SITES_UPDATE = "storage_sites.update";

        /// <summary>
        /// The right that allows to create a new storage site.
        /// </summary>
        public static readonly string STORAGE_AREAS_CREATE = "storage_areas.create";

        /// <summary>
        /// The right that allows to update a storage area.
        /// </summary>
        public static readonly string STORAGE_AREAS_UPDATE = "storage_areas.update";

        #endregion
    }
}
