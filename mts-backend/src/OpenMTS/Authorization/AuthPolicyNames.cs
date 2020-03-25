namespace OpenMTS.Authorization
{
    /// <summary>
    /// An enumeration of all authorization policy names.
    /// </summary>
    public static class AuthPolicyNames
    {
        #region Materials

        /// <summary>
        /// The policy that authorizes the creation of materials.
        /// </summary>
        public const string MAY_CREATE_MATERIAL = "MayCreateMaterial";

        /// <summary>
        /// The policy that authorizes the updating of materials.
        /// </summary>
        public const string MAY_UPDATE_MATERIAL = "MayUpdateMaterial";

        /// <summary>
        /// The policy that authorizes the setting of custom material prop values.
        /// </summary>
        public const string MAY_SET_CUSTOM_MATERIAL_PROP_VALUE = "MaySetCustomMaterialPropValue";

        /// <summary>
        /// The policy that authorizes the deletion of custom material prop values.
        /// </summary>
        public const string MAY_DELETE_CUSTOM_MATERIAL_PROP_VALUE = "MayDeleteCustomMaterialPropValue";

        #endregion

        // TODO: add policy names for material types

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
    }
}
