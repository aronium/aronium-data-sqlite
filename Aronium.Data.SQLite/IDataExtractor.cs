using System.Collections.Generic;
using System.Data;

namespace Aronium.Data.SQLite
{
    /// <summary>
    /// Extracts data of specified type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">Type to extract from data reader.</typeparam>
    public interface IDataExtractor<T>
    {
        /// <summary>
        /// Extracts data from specified data reader.
        /// </summary>
        /// <param name="reader">Reader from which data is extracted.</param>
        /// <returns>List of specified type.</returns>
        IEnumerable<T> Extract(IDataReader reader);
    }
}
