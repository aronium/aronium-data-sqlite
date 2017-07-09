using System.Data.SQLite;

namespace Aronium.Data.SQLite
{
    /// <summary>
    /// Serves as a contract for RowMapper implementations.
    /// </summary>
    /// <typeparam name="T">Entity type to return.</typeparam>
    public interface IRowMapper<T>
    {
        /// <summary>
        /// Maps data record to entity type.
        /// </summary>
        /// <param name="record">Data record.</param>
        /// <returns>Instance of {T}.</returns>
        T Map(SQLiteDataReader record);
    }
}
