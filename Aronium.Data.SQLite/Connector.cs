using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;

namespace Aronium.Data.SQLite
{
    public class Connector : IDisposable
    {
        #region - Fields -
        private string _dataFile;
        private string _connectionString;

        /// <summary>
        /// Gets or sets connection string.
        /// </summary>
        public string ConnectionString
        {
            get
            {
                if (string.IsNullOrEmpty(_connectionString))
                {
                    _connectionString = string.Format("data source={0};foreign keys=true;", this.DataFile);
                }
                return _connectionString;
            }
            set { _connectionString = value; }
        }
        #endregion

        #region - Constructors -

        /// <summary>
        /// Initializes new instance of Connector class with specified data file.
        /// </summary>
        /// <param name="dataFile">SQLite database file path.</param>
        public Connector(string dataFile)
        {
            if (dataFile == null)
            {
                throw new ArgumentNullException("dataFile");
            }

            this.DataFile = dataFile;
        }

        #endregion

        #region - Properties -

        /// <summary>
        /// Gets database file name.
        /// <para>Property will be set upon successfull connect.</para>
        /// </summary>
        public string DataFile
        {
            get { return _dataFile; }
            private set
            {
                _dataFile = value;
            }
        }

        #endregion

        #region - Private methods -

        private void PrepareCommandParameters(SQLiteCommand command, IEnumerable<SQLiteQueryParameter> queryParameters)
        {
            if (queryParameters != null)
            {
                var commandParameters = queryParameters.Select(x => new SQLiteParameter(x.Name, x.Value));

                command.Parameters.AddRange(commandParameters.ToArray());
            }
        }

        #endregion

        #region - Public methods -

        /// <summary>
        /// Attempts to connect to database.
        /// </summary>
        /// <param name="fileName">Database file name.</param>
        /// <returns>True if connected successfully.</returns>
        public bool Connect(string fileName)
        {
            using (var connection = new SQLiteConnection(string.Format("Data Source={0};Version=3;", fileName)))
            {
                connection.Open();

                connection.Close();

                DataFile = fileName;

                return true;
            }
        }

        /// <summary>
        /// Executes query against in current database.
        /// </summary>
        /// <param name="query">Command text.</param>
        /// <param name="queryParameters">Query parameters.</param>
        public int Execute(string query, IEnumerable<SQLiteQueryParameter> queryParameters = null)
        {
            using (var connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();
                
                using (SQLiteCommand command = connection.CreateCommand())
                {
                    command.CommandText = query;

                    if (queryParameters != null)
                    {
                        var commandParameters = queryParameters.Select(x => new SQLiteParameter(x.Name, x.Value));

                        command.Parameters.AddRange(commandParameters.ToArray());
                    }

                    return command.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Gets entity instance.
        /// </summary>
        /// <typeparam name="T">Type of object to create</typeparam>
        /// <param name="query">Sql Query</param>
        /// <param name="args">Sql Parameters</param>
        /// <returns>Entity instance.</returns>
        /// <remarks>Instance properties are populated from database record using reflection for the given type.</remarks>
        public T SelectSingle<T>(string query, IEnumerable<SQLiteQueryParameter> queryParameters = null) where T : class, new()
        {
            T entity = null;

            using (var connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();

                using (SQLiteCommand command = connection.CreateCommand())
                {
                    command.CommandText = query;

                    PrepareCommandParameters(command, queryParameters);

                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        reader.Read();

                        if (reader.HasRows)
                        {
                            entity = new T();
                            var type = typeof(T);

                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                var val = reader[i];

                                var property = type.GetProperty(reader.GetName(i));

                                // SQLite stores primary keys as Int64
                                // Standard implementation uses int as id field, which must be converted from Int64 to Int32
                                if (val is long && property.PropertyType == typeof(Int32))
                                {
                                    val = Convert.ToInt32(val);
                                }

                                if (val is string && property.PropertyType == typeof(Guid))
                                {
                                    val = new Guid((string)val);
                                }

                                if (property != null)
                                {
                                    property.SetValue(entity, val == Convert.DBNull ? null : val, null);
                                }
                            }
                        }

                        reader.Close();
                    }
                }
            }

            return entity;
        }

        /// <summary>
        /// Gets list of entities.
        /// </summary>
        /// <typeparam name="T">Type of object to create</typeparam>
        /// <param name="query">Sql Query</param>
        /// <param name="queryParameters">Sql query parameters</param>
        /// <returns>List of entities.</returns>
        /// <remarks>Instance properties are populated from database record using reflection for the speecified type.</remarks>
        public IEnumerable<T> Select<T>(string query, IEnumerable<SQLiteQueryParameter> queryParameters = null) where T : class, new()
        {
            using (var connection = new SQLiteConnection(ConnectionString))
            {
                var type = typeof(T);

                connection.Open();

                using (SQLiteCommand command = connection.CreateCommand())
                {
                    command.CommandText = query;

                    PrepareCommandParameters(command, queryParameters);

                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            T entity = new T();

                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                var val = reader[i];

                                var property = type.GetProperty(reader.GetName(i));

                                // SQLite stores primary keys as Int64
                                // Standard implementation uses int as id field, which must be converted from Int64 to Int32
                                if (val is long && property.PropertyType == typeof(Int32))
                                {
                                    val = Convert.ToInt32(val);
                                }

                                if (val is string && property.PropertyType == typeof(Guid))
                                {
                                    val = new Guid((string)val);
                                }

                                if (property != null)
                                {
                                    property.SetValue(entity, val == Convert.DBNull ? null : val, null);
                                }
                            }

                            yield return entity;
                        }

                        reader.Close();
                    }
                }
            }
        }

        /// <summary>
        /// Execute reader and create list of provided type using IRowMapper interface.
        /// </summary>
        /// <typeparam name="T">Type of object to create.</typeparam>
        /// <param name="query">Sql Query.</param>
        /// <param name="queryParameters">Sql query parameters.</param>
        /// <param name="rowMapper">IRowMapper used to map object instance from reader.</param>
        /// <returns>List of provided object type.</returns>
        public IEnumerable<T> Select<T>(string query, IEnumerable<SQLiteQueryParameter> queryParameters, IRowMapper<T> rowMapper)
        {
            using (var connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();

                using (SQLiteCommand command = connection.CreateCommand())
                {
                    command.CommandText = query;

                    PrepareCommandParameters(command, queryParameters);

                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        var isNullable = Nullable.GetUnderlyingType(typeof(T)) != null;

                        while (reader.Read())
                        {
                            if (rowMapper != null)
                            {
                                yield return rowMapper.Map(reader);
                            }
                            else
                            {
                                // Check for null values and return default instance of T (should be nullable)
                                // If not checked for NULL values, conversion will fail, resulting in InvalidCastException being thrown
                                if (isNullable && reader[0] == Convert.DBNull)
                                {
                                    yield return default(T);
                                }
                                else
                                    yield return (T)reader[0];
                            }
                        }

                        reader.Close();
                    }
                }
            }
        }

        private static readonly string CHECK_TABLE_EXISTS = "SELECT name FROM sqlite_master WHERE type='table' AND name=:TableName;";

        public bool TableExists(string tableName)
        {
            bool exists = false;

            using (var connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();

                using (SQLiteCommand command = connection.CreateCommand())
                {
                    command.CommandText = CHECK_TABLE_EXISTS;

                    command.Parameters.AddWithValue("TableName", tableName);

                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        reader.Read();

                        exists = reader.HasRows;

                        reader.Close();
                    }
                }
            }

            return exists;
        }

        /// <summary>
        /// Disposes this instance.
        /// </summary>
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        } 

        #endregion
    }
}
