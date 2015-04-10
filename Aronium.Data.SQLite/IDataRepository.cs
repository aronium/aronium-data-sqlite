using System;
using System.Collections.Generic;

namespace Aronium.Data.SQLite
{
    public interface IDataRepository
    {
        string DataFile { get; set; }
    }
}
