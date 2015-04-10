using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aronium.Data.SQLite
{
    /// <summary>
    /// Contains methods for handling database operations for specified type.
    /// </summary>
    /// <typeparam name="TEntity">Entity type.</typeparam>
    public abstract class DataRepository<TEntity> : IDisposable where TEntity : class, new()
    {
        #region - Fields -

        private bool _disposed;
        private Type _type;
        private string _selectQuery;

        private static readonly string INSERT = "INSERT INTO [{0}] ({1}) VALUES ({2})";
        private static readonly string SELECT = "SELECT {0} FROM [{1}]";
        private static readonly string DELETE = "DELETE FROM [{0}] WHERE ID=:ID";
        private static readonly string SELECT_BY_ID = "SELECT {0} FROM [{1}] WHERE ID=:ID";

        #endregion

        #region - Properties -

        /// <summary>
        /// Gets {TEntity} entity type.
        /// </summary>
        public Type EntityType
        {
            get
            {
                if (_type == null)
                    _type = typeof(TEntity);
                return _type;
            }
        }

        /// <summary>
        /// Gets select query for entity type.
        /// </summary>
        protected string SelectQueryString
        {
            get
            {
                if (_selectQuery == null)
                {
                    var properties = GetEntityPropertyNames();

                    var columns = string.Join(",", properties.Select(property => string.Format("[{0}]", property)));

                    _selectQuery = string.Format(SELECT, columns, EntityType.Name);
                }

                return _selectQuery;
            }
        }

        #endregion

        #region - Private methods -

        /// <summary>
        /// Gets list of property names.
        /// </summary>
        /// <returns>List of property names.</returns>
        private IEnumerable<string> GetEntityPropertyNames()
        {
            return EntityType.GetProperties().Where(x => x.CanWrite && !x.GetSetMethod().IsVirtual).Select(x => x.Name);
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
        /// Occurs before entity insert.
        /// </summary>
        /// <param name="entity">Entity instance to insert.</param>
        protected virtual void OnBeforeInsert(TEntity entity) { }

        /// <summary>
        /// Occurs after entity is inserted.
        /// </summary>
        /// <param name="entity">Entity instance inserted.</param>
        protected virtual void OnAfterInsert(TEntity entity) { }

        /// <summary>
        /// Occurs after entity is selected.
        /// </summary>
        /// <param name="entity">Selected entity instance.</param>
        protected virtual void OnAfterSelectEntity(TEntity entity) { }

        /// <summary>
        /// Occurs before entity is deleted.
        /// </summary>
        /// <param name="entity"></param>
        protected virtual void OnBeforeDelete(object id) { }

        /// <summary>
        /// Occurs after entity is deleted.
        /// </summary>
        /// <param name="entity"></param>
        protected virtual void OnAfterDelete(object id) { }

        #endregion

        #region - Public methods -

        /// <summary>
        /// Inserts entity.
        /// </summary>
        /// <param name="entity">Entity to insert.</param>
        /// <returns>True if inserted succesfully, otherwise false.</returns>
        public bool Insert(TEntity entity)
        {
            using (var connector = new Connector())
            {
                var properties = GetEntityPropertyNames();

                var columns = string.Join(",", properties.Select(property => string.Format("[{0}]", property)));
                var arguments = string.Join(",", properties.Select(property => string.Format(":{0}", property)));

                var sql = string.Format(INSERT, EntityType.Name, columns, arguments);

                List<SQLiteQueryParameter> parameters = new List<SQLiteQueryParameter>();

                OnBeforeInsert(entity);

                foreach (var prop in properties)
                {
                    var propertyValue = EntityType.GetProperty(prop).GetValue(entity, null);
                    if (propertyValue == null)
                    {
                        propertyValue = Convert.DBNull;
                    }
                    else if (propertyValue.GetType().IsEnum)
                    {
                        propertyValue = (int)propertyValue;
                    }

                    parameters.Add(new SQLiteQueryParameter(prop, propertyValue));
                }

                var rowsAffected = connector.Execute(sql, parameters);

                OnAfterInsert(entity);

                return rowsAffected > 0;
            }
        }

        /// <summary>
        /// Gets list of all entities.
        /// </summary>
        /// <returns>Entity list.</returns>
        public IEnumerable<TEntity> All()
        {
            using (var connector = new Connector())
            {
                foreach (var entity in connector.Select<TEntity>(SelectQueryString))
                {
                    OnAfterSelectEntity(entity);

                    yield return entity;
                }
            }
        }

        /// <summary>
        /// Finds entity by ID.
        /// </summary>
        /// <param name="id">Entity ID.</param>
        /// <returns>Entity instance.</returns>
        public TEntity GetById(object id)
        {
            using (var connector = new Connector())
            {
                var properties = GetEntityPropertyNames();

                var columns = string.Join(",", properties.Select(property => string.Format("[{0}]", property)));

                var sql = string.Format(SELECT_BY_ID, columns, EntityType.Name);

                var entity = connector.SelectSingle<TEntity>(sql, new[] { 
                    new SQLiteQueryParameter("ID", id) 
                });

                OnAfterSelectEntity(entity);

                return entity;
            }
        }

        /// <summary>
        /// Deletes entity.
        /// </summary>
        /// <param name="id">Entity id.</param>
        public void Delete(object id)
        {
            using (var connector = new Connector())
            {
                var sql = string.Format(DELETE, EntityType.Name);

                OnBeforeDelete(id);

                connector.Execute(sql, new[] { new SQLiteQueryParameter(":ID", id) });

                OnAfterDelete(id);
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

        #endregion
    }
}
