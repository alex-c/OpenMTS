namespace OpenMTS.Authorization
{
    /// <summary>
    /// An enumeration of all authorization policy names.
    /// </summary>
    public static class AuthPolicyNames
    {
        #region OpenMTS configuration

        /// <summary>
        /// The policy that authorizes configuration setting.
        /// </summary>
        public const string MAY_SET_CONFIGURATION = "MaySetConfiguration";

        /// <summary>
        /// The policy that authorizes the creation of custom material props.
        /// </summary>
        public const string MAY_CREATE_CUSTOM_MATERIAL_PROP = "MayCreateCustomMaterialProp";

        /// <summary>
        /// The policy that authorizes the uptating of custom material props.
        /// </summary>
        public const string MAY_UPDATE_CUSTOM_MATERIAL_PROP = "MayUpdateCustomMaterialProp";

        /// <summary>
        /// The policy that authorizes the deletion of custom material props.
        /// </summary>
        public const string MAY_DELETE_CUSTOM_MATERIAL_PROP = "MayDeleteCustomMaterialProp";

        #endregion

        #region User administration

        /// <summary>
        /// The policy that authorizes user creation.
        /// </summary>
        public const string MAY_CREATE_USER = "MayCreateUser";

        /// <summary>
        /// The policy that authorizes user updating.
        /// </summary>
        public const string MAY_UPDATE_USER = "MayUpdateUser";

        /// <summary>
        /// The policy that authorizes user status updating.
        /// </summary>
        public const string MAY_UPDATE_USER_STATUS = "MayUpdateUserStatus";

        #endregion

        #region API key administration

        /// <summary>
        /// The policy that authorizes the querying of existing API keys.
        /// </summary>
        public const string MAY_QUERY_KEYS = "MayQueryKeys";

        /// <summary>
        /// The policy that authorizes key creation.
        /// </summary>
        public const string MAY_CREATE_KEY = "MayCreateKey";

        /// <summary>
        /// The policy that authorizes key updating.
        /// </summary>
        public const string MAY_UPDATE_KEY = "MayUpdateKey";

        /// <summary>
        /// The policy that authorizes key status updating.
        /// </summary>
        public const string MAY_UPDATE_KEY_STATUS = "MayUpdateKeyStatus";

        /// <summary>
        /// The policy that authorizes key deletion.
        /// </summary>
        public const string MAY_DELETE_KEY = "MayDeleteKey";

        #endregion

        #region Locations administration

        /// <summary>
        /// The policy that authorizes storage site creation.
        /// </summary>
        public const string MAY_CREATE_STORAGE_SITE = "MayCreateStorageSite";

        /// <summary>
        /// The policy that authorizes storage site updating.
        /// </summary>
        public const string MAY_UPDATE_STORAGE_SITE = "MayUpdateStorageSite";

        /// <summary>
        /// The policy that authorizes storage area creation.
        /// </summary>
        public const string MAY_CREATE_STORAGE_AREA = "MayCreateStorageArea";

        /// <summary>
        /// The policy that authorizes storage area updating.
        /// </summary>
        public const string MAY_UPDATE_STORAGE_AREA = "MayUpdateStorageArea";

        #endregion

        // TODO: add missing policy names
        //  - material types (do with 'plastics' renaming)
        //  - materials
    }
}
