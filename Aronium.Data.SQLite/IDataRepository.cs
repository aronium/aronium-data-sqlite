namespace Aronium.Data.SQLite
{
    /// <summary>
    /// Entity repository contract.
    /// </summary>
    public interface IDataRepository
    {
        /// <summary>
        /// Database file path.
        /// </summary>
        string DataFile { get; set; }
    }
}
