using System.Data.SQLite;

namespace Aronium.Data.SQLite
{
    public interface IRowMapper<T>
    {
        /// <summary>
        /// Maps data record to entity type.s
        /// </summary>
        /// <param name="record">Data record.</param>
        /// <returns>Instance of {T}.</returns>
        T Map(SQLiteDataReader record);
    }
}
