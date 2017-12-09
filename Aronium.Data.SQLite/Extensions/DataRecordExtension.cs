using System;
using System.Data;

namespace Aronium.Data.SQLite
{
    /// <summary>
    /// DataRecord extension class.
    /// </summary>
    public static class DataRecordExtension
    {
        /// <summary>
        /// Gets string or null value if record is <see cref="DBNull"/>.
        /// </summary>
        /// <param name="record">Data record.</param>
        /// <param name="ordinal">Column index.</param>
        /// <returns>String value or null.</returns>
        public static string GetNullableString(this IDataRecord record, int ordinal)
        {
            return record.IsDBNull(ordinal) ? null : record.GetString(ordinal);
        }

        /// <summary>
        /// Gets GUID or null value if record is <see cref="DBNull"/>.
        /// </summary>
        /// <param name="record">Data record.</param>
        /// <param name="ordinal">Column index.</param>
        /// <returns>String value or null.</returns>
        public static Guid? GetNullableGuid(this IDataRecord record, int ordinal)
        {
            return record.IsDBNull(ordinal) ? default(Guid?) : record.GetGuid(ordinal);
        }


        /// <summary>
        /// Gets int value or zero if record is <see cref="DBNull"/>.
        /// </summary>
        /// <param name="record">Data record.</param>
        /// <param name="ordinal">Column index.</param>
        /// <returns>String value or null.</returns>
        public static int GetInt32OrZero(this IDataRecord record, int ordinal)
        {
            return record.IsDBNull(ordinal) ? 0 : record.GetInt32(ordinal);
        }

        /// <summary>
        /// Gets nullable int value..
        /// </summary>
        /// <param name="record">Data record.</param>
        /// <param name="ordinal">Column index.</param>
        /// <returns>String value or null.</returns>
        public static int? GetNullableInt32(this IDataRecord record, int ordinal)
        {
            return record.IsDBNull(ordinal) ? default(int?) : record.GetInt32(ordinal);
        }

        /// <summary>
        /// Gets decimal value or null if record is <see cref="DBNull"/>.
        /// </summary>
        /// <param name="record">Data record.</param>
        /// <param name="ordinal">Column index.</param>
        /// <returns>String value or null.</returns>
        public static decimal GetDecimalOrZero(this IDataRecord record, int ordinal)
        {
            return record.IsDBNull(ordinal) ? 0M : record.GetDecimal(ordinal);
        }

        /// <summary>
        /// Gets nullable date time value.
        /// </summary>
        /// <param name="record">Data record.</param>
        /// <param name="ordinal">Column index.</param>
        /// <returns>String value or null.</returns>
        public static DateTime? GetNullableDateTime(this IDataRecord record, int ordinal)
        {
            return record.IsDBNull(ordinal) ? default(DateTime?) : record.GetDateTime(ordinal);
        }

        /// <summary>
        /// Gets value of specified type <typeparamref name="T"/> or default if record is <see cref="DBNull"/>.
        /// </summary>
        /// <param name="record">Data record.</param>
        /// <param name="fieldName">Column name.</param>
        /// <returns>Data record value or default.</returns>
        public static T GetValueOrDefault<T>(this IDataRecord record, string fieldName)
        {
            return record.GetValueOrDefault<T>(record.GetOrdinal(fieldName));
        }

        /// <summary>
        /// Gets value of specified type <typeparamref name="T"/> or default if record is <see cref="DBNull"/>.
        /// </summary>
        /// <param name="record">Data record.</param>
        /// <param name="ordinal">Column index.</param>
        /// <returns>Data record value or default.</returns>
        public static T GetValueOrDefault<T>(this IDataRecord record, int ordinal)
        {
            return (T)(record.IsDBNull(ordinal) ? default(T) : record.GetValue(ordinal));
        }
    }
}
