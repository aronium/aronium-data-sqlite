using System.Collections.Generic;

namespace Aronium.Data.SQLite
{
    public class SQLiteQueryParameter
    {
        /// <summary>
        /// Initializes new instance of SQLitequeryParameter class.
        /// </summary>
        public SQLiteQueryParameter() { }

        /// <summary>
        /// Initializes new instance of SQLitequeryParameter class with specified name and value.
        /// </summary>
        /// <param name="name">Query parameter name.</param>
        /// <param name="value">Query parameter value.</param>
        public SQLiteQueryParameter(string name, object value)
        {
            this.Name = name;
            this.Value = value;
        }

        /// <summary>
        /// Gets or sets parameter name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets value.
        /// </summary>
        public object Value { get; set; }

        /// <summary>
        /// Gets query parameter array with single parameter value.
        /// </summary>
        /// <param name="name">Parameter name.</param>
        /// <param name="value">Parameter value.</param>
        /// <returns>Query parameter array containing instance with specified name and value.</returns>
        public static IEnumerable<SQLiteQueryParameter> Single(string name, object value)
        {
            return new[] { new SQLiteQueryParameter(name, value) };
        }
    }
}
