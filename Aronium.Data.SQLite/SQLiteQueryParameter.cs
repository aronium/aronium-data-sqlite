using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aronium.Data.SQLite
{
    public class SQLiteQueryParameter
    {
        public SQLiteQueryParameter() { }

        public SQLiteQueryParameter(string name, object value)
        {
            this.Name = name;
            this.Value = value;
        }

        public string Name { get; set; }

        public object Value { get; set; }

        public bool IsOutput { get; set; }
    }
}
