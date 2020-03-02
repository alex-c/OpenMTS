namespace OpenMTS.Authorization
{
    /// <summary>
    /// An enumeration of all authorization policy names.
    /// </summary>
    public static class AuthPolicyNames
    {
        /// <summary>
        /// The policy that authorizes configuration setting.
        /// </summary>
        public const string MAY_SET_CONFIGURATION = "MaySetConfiguration";

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
    }
}
