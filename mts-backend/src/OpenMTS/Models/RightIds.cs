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
                CONFIGFURATION_SET,
                USERS_CREATE,
                USERS_UPDATE,
                USERS_UPDATE_STATUS,
                KEYS_CREATE,
                KEYS_UPDATE,
                KEYS_UPDATE_STATUS,
                KEYS_DELETE

                // TODO: add missing rights
            };
        }

        /// <summary>
        /// The right that allows to set the OpenMTS configuration.
        /// </summary>
        public static readonly string CONFIGFURATION_SET = "configuration.set";

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
