using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aronium.Data.SQLite
{
    /// <summary>
    /// Class used as a data repository.
    /// </summary>
    public class Repository : IDisposable, IDataRepository
    {
        #region - Fields -
        private bool _disposed;
        #endregion

        #region - Properties -

        /// <summary>
        /// Gets SQLite database file path.
        /// </summary>
        public virtual string DataFile { get; set; }

        #endregion

        #region - Private methods -

        private IEnumerable<T> GetListUsingRowMapper<T>(string query, IEnumerable<SQLiteQueryParameter> args, IRowMapper<T> rowMapper)
        {
            using (var connector = new Connector(DataFile))
            {
                return connector.Select(query, args, rowMapper);
            }
        }

        private IEnumerable<T> GetListUsingDataExtractor<T>(string query, IEnumerable<SQLiteQueryParameter> args, IDataExtractor<T> extractor)
        {
            using (var connector = new Connector(DataFile))
            {
                return connector.Select(query, args, extractor);
            }
        }

        #endregion

        #region - Protected methods -

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
            return GetListUsingRowMapper<T>(query, null, null);
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
            return GetListUsingRowMapper<T>(query, args, null);
        }

        /// <summary>
        /// Gets list of specified type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">Type to instantiate.</typeparam>
        /// <param name="query">SQL query.</param>
        /// <param name="args">SQL query arguments.</param>
        /// <param name="rowMapper">Row mapper instance used to instantiate specified type.</param>
        /// <returns>List of instances of type <typeparamref name="T"/>.</returns>
        protected IEnumerable<T> GetList<T>(string query, IEnumerable<SQLiteQueryParameter> args, IRowMapper<T> rowMapper)
        {
            return GetListUsingRowMapper(query, args, rowMapper);
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
            return GetListUsingDataExtractor(query, args, extractor);
        }

        /// <summary>
        /// Executes specified SQL query and returns number of rows affected.
        /// </summary>
        /// <param name="query">SQL query.</param>
        /// <returns>Number of rows affected by the query execution.</returns>
        protected int Execute(string query)
        {
            return Execute(query, null);
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
        /// Executes specified SQL query and returns number of rows affected assigning last rowid to specifed out parameter.
        /// </summary>
        /// <param name="query">SQL query.</param>
        /// <param name="args">SQL query arguments.</param>
        /// <param name="rowId">Out parameter to assign last inserted row id.</param>
        /// <returns>Number of rows affected by the query execution.</returns>
        protected int Execute(string query, IEnumerable<SQLiteQueryParameter> args, out long rowId)
        {
            using (var connector = new Connector(DataFile))
                return connector.Execute(query, args, out rowId);
        }

        /// <summary>
        /// Execute prepared commands in single transaction.
        /// </summary>
        /// <param name="commands">Commands to execute in transaction.</param>
        protected void Execute(IEnumerable<PreparedCommand> commands)
        {
            using (var connector = new Connector(DataFile))
                connector.Execute(commands);
        }

        #endregion

        #region - Public methods -

        /// <summary>
        /// Dispose object.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);

            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
