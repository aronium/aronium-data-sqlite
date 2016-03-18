using System.Collections.Generic;

namespace Aronium.Data.SQLite
{
    public class PreparedCommand
    {
        public PreparedCommand() { }

        public PreparedCommand(string commandText) : this()
        {
            CommandText = commandText;
        }
        public PreparedCommand(string commandText, IEnumerable<SQLiteQueryParameter> args) : this(commandText)
        {
            QueryArguments = args;
        }

        public string CommandText { get; set; }

        public IEnumerable<SQLiteQueryParameter> QueryArguments { get; set; }
    }
}
