using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aronium.Data.SQLite
{
    public class Repository : IDisposable, IDataRepository
    {
        private bool _disposed;

        /// <summary>
        /// Gets SQLIte database file path.
        /// </summary>
        public string DataFile { get; set; }

        private IEnumerable<T> GetListWithRowMapper<T>(string query, IEnumerable<SQLiteQueryParameter> args, IRowMapper<T> rowMapper)
        {
            using (var connector = new Connector(DataFile))
            {
                return connector.Select(query, args, rowMapper);
            }
        }

        private IEnumerable<T> GetListWithDataExtractor<T>(string query, IEnumerable<SQLiteQueryParameter> args, IDataExtractor<T> extractor)
        {
            using (var connector = new Connector(DataFile))
            {
                return connector.Select(query, args, extractor);
            }
        }

        /// <summary>
        /// Dispose object.
        /// </summary>
        /// <param name="disposing">Value indicating whether disposal is in progress.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    // Dispose objects...
                }
            }

            _disposed = true;
        }

        /// <summary>
        /// Gets instance of specified type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">Type to instantiate.</typeparam>
        /// <param name="query">SQL query.</param>
        /// <returns>Instance of <typeparamref name="T"/></returns>
        protected T Get<T>(string query)
        {
            return Get<T>(query, null, null);
        }

        /// <summary>
        /// Gets instance of specified type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">Type to instantiate.</typeparam>
        /// <param name="query">SQL query.</param>
        /// <param name="args">SQL query arguments.</param>
        /// <returns>Instance of <typeparamref name="T"/></returns>
        protected T Get<T>(string query, IEnumerable<SQLiteQueryParameter> args)
        {
            return Get<T>(query, args, null);
        }

        /// <summary>
        /// Gets instance of specified type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">Type to instantiate.</typeparam>
        /// <param name="query">SQL query.</param>
        /// <param name="rowMapper">Row mapper instance used to instantiate specified type.</param>
        /// <returns>Instance of <typeparamref name="T"/></returns>
        protected T Get<T>(string query, IRowMapper<T> rowMapper)
        {
            return Get(query, null, rowMapper);
        }

        /// <summary>
        /// Gets instance of specified type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">Type to instantiate.</typeparam>
        /// <param name="query">SQL query.</param>
        /// <param name="args">SQL query arguments.</param>
        /// <param name="rowMapper">Row mapper instance used to instantiate specified type.</param>
        /// <returns>Instance of <typeparamref name="T"/></returns>
        protected T Get<T>(string query, IEnumerable<SQLiteQueryParameter> args, IRowMapper<T> rowMapper)
        {
            using (var connector = new Connector(DataFile))
            {
                return connector.SelectValue(query, args, rowMapper);
            }
        }

        /// <summary>
        /// Gets list of specified type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">Type to instantiate.</typeparam>
        /// <param name="query">SQL query.</param>
        /// <returns>List of instances of type <typeparamref name="T"/>.</returns>
        protected IEnumerable<T> GetList<T>(string query)
        {
            return GetListWithRowMapper<T>(query, null, null);
        }

        /// <summary>
        /// Gets list of specified type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">Type to instantiate.</typeparam>
        /// <param name="query">SQL query.</param>
        /// <param name="rowMapper">Row mapper instance used to instantiate specified type.</param>
        /// <returns>List of instances of type <typeparamref name="T"/>.</returns>
        protected IEnumerable<T> GetList<T>(string query, IRowMapper<T> rowMapper)
        {
            return GetList<T>(query, null, rowMapper);
        }

        /// <summary>
        /// Gets list of specified type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">Type to instantiate.</typeparam>
        /// <param name="query">SQL query.</param>
        /// <param name="args">SQL query arguments.</param>
        /// <returns>List of instances of type <typeparamref name="T"/>.</returns>
        protected IEnumerable<T> GetList<T>(string query, IEnumerable<SQLiteQueryParameter> args)
        {
            return GetListWithRowMapper<T>(query, args, null);
        }

        /// <summary>
        /// Gets list of specified type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">Type to instantiate.</typeparam>
        /// <param name="query">SQL query.</param>
        /// <param name="args">SQL query arguments.</param>
        /// <param name="extractor">Row mapper instance used to instantiate specified type.</param>
        /// <returns>List of instances of type <typeparamref name="T"/>.</returns>
        protected IEnumerable<T> GetList<T>(string query, IEnumerable<SQLiteQueryParameter> args, IRowMapper<T> rowMapper)
        {
            return GetListWithRowMapper(query, args, rowMapper);
        }

        /// <summary>
        /// Gets list of specified type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">Type to instantiate.</typeparam>
        /// <param name="query">SQL query.</param>
        /// <param name="extractor">Data extractor instance used to instantiate specified type.</param>
        /// <returns>List of instances of type <typeparamref name="T"/>.</returns>
        protected IEnumerable<T> GetList<T>(string query, IDataExtractor<T> extractor)
        {
            return GetList<T>(query, null, extractor);
        }

        /// <summary>
        /// Gets list of specified type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">Type to instantiate.</typeparam>
        /// <param name="query">SQL query.</param>
        /// <param name="args">SQL query arguments.</param>
        /// <param name="extractor">Data extractor instance used to instantiate specified type.</param>
        /// <returns>List of instances of type <typeparamref name="T"/>.</returns>
        protected IEnumerable<T> GetList<T>(string query, IEnumerable<SQLiteQueryParameter> args, IDataExtractor<T> extractor)
        {
            return GetListWithDataExtractor(query, args, extractor);
        }

        /// <summary>
        /// Executes specified SQL query and returns number of rows affected.
        /// </summary>
        /// <param name="query">SQL query.</param>
        /// <param name="args">SQL query arguments.</param>
        /// <returns>Number of rows affected by the query execution.</returns>
        protected int Execute(string query, IEnumerable<SQLiteQueryParameter> args)
        {
            using (var connector = new Connector(DataFile))
            {
                return connector.Execute(query, args);
            }
        }

        /// <summary>
        /// Dispose object.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);

            GC.SuppressFinalize(this);
        }
    }
}
