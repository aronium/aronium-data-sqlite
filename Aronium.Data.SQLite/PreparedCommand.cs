using System.Collections.Generic;

namespace Aronium.Data.SQLite
{
    /// <summary>
    /// Prepared SQL command.
    /// </summary>
    public class PreparedCommand
    {
        /// <summary>
        /// Initializes new instance of PreparedCommand class.
        /// </summary>
        public PreparedCommand() { }

        /// <summary>
        /// Initializes new instance of PreparedCommand class with specified command text.
        /// </summary>
        /// <param name="commandText">SQL Query used as command text.</param>
        public PreparedCommand(string commandText) : this()
        {
            CommandText = commandText;
        }

        /// <summary>
        /// Initializes new instance of PreparedCommand class with specified SQL query and query parameters.
        /// </summary>
        /// <param name="commandText">SQL Query used as command text.</param>
        /// <param name="args">SQL auery parameters.</param>
        public PreparedCommand(string commandText, IEnumerable<SQLiteQueryParameter> args) : this(commandText)
        {
            QueryArguments = args;
        }

        /// <summary>
        /// Gets or sets command text.
        /// </summary>
        public string CommandText { get; set; }

        /// <summary>
        /// Gets or sets query parameters.
        /// </summary>
        public IEnumerable<SQLiteQueryParameter> QueryArguments { get; set; }
    }
}
