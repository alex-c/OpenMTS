using Dapper;
using System;
using System.Data;

namespace OpenMTS.Repositories.PostgreSQL.Support
{
    /// <summary>
    /// A custom Dapper SQL mapper type handler for <see cref="System.DateTime"/> to handle UTC, 
    /// </summary>
    /// <seealso cref="Dapper.SqlMapper.TypeHandler{System.DateTime}" />
    public class DateTimeHandler : SqlMapper.TypeHandler<DateTime>
    {
        /// <summary>
        /// Assign the value of a parameter before a command executes
        /// </summary>
        /// <param name="parameter">The parameter to configure</param>
        /// <param name="value">Parameter value</param>
        public override void SetValue(IDbDataParameter parameter, DateTime value)
        {
            parameter.Value = value;
        }

        /// <summary>
        /// Parse a database value back to a typed value
        /// </summary>
        /// <param name="value">The value from the database</param>
        /// <returns>
        /// The typed value
        /// </returns>
        public override DateTime Parse(object value)
        {
            return DateTime.SpecifyKind((DateTime)value, DateTimeKind.Utc);
        }
    }
}
