using System;
using System.Data;

namespace Aronium.Data.SQLite
{
    public static class DataRecordExtension
    {
        public static string GetNullableString(this IDataRecord record, int ordinal)
        {
            return record.IsDBNull(ordinal) ? null : record.GetString(ordinal);
        }

        public static Guid? GetNullableGuid(this IDataRecord record, int ordinal)
        {
            return record.IsDBNull(ordinal) ? default(Guid?) : record.GetGuid(ordinal);
        }

        public static int GetInt32OrZero(this IDataRecord record, int ordinal)
        {
            return record.IsDBNull(ordinal) ? 0 : record.GetInt32(ordinal);
        }

        public static int? GetNullableInt32(this IDataRecord record, int ordinal)
        {
            return record.IsDBNull(ordinal) ? default(int?) : record.GetInt32(ordinal);
        }

        public static decimal GetDecimalOrZero(this IDataRecord record, int ordinal)
        {
            return record.IsDBNull(ordinal) ? 0M : record.GetDecimal(ordinal);
        }

        public static T GetValueOrDefault<T>(this IDataRecord record, string fieldName)
        {
            return record.GetValueOrDefault<T>(record.GetOrdinal(fieldName));
        }

        public static T GetValueOrDefault<T>(this IDataRecord record, int ordinal)
        {
            return (T)(record.IsDBNull(ordinal) ? default(T) : record.GetValue(ordinal));
        }
    }
}
