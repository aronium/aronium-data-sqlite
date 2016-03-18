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
    public abstract class DataRepository<TEntity> : Repository, IDataRepository where TEntity : class, new()
    {
        #region - Fields -

        private bool _disposed;
        private Type _type;
        private string _selectQuery;

        private static readonly string INSERT = "INSERT INTO [{0}] ({1}) VALUES ({2})";
        private static readonly string SELECT = "SELECT {0} FROM [{1}]";
        private static readonly string DELETE = "DELETE FROM [{0}] WHERE ID=:ID";
        private static readonly string SELECT_BY_ID = "SELECT {0} FROM [{1}] WHERE ID=:ID";
        private static readonly string SELECT_CURRENT_SEQUENCE = "SELECT seq FROM SQLITE_SEQUENCE WHERE name='{0}'";

        #endregion

        #region - Private properties -

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

        #endregion

        #region - Protected methods -

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

        /// <summary>
        /// Gets current sequence for table.
        /// </summary>
        /// <returns>Current autoincrement sequence value.</returns>
        protected long GetCurrentSequence()
        {
            return Get<long>(string.Format(SELECT_CURRENT_SEQUENCE, EntityType.Name));
        }

        #endregion

        #region - Public methods -

        /// <summary>
        /// Inserts entity.
        /// </summary>
        /// <param name="entity">Entity to insert.</param>
        /// <returns>True if inserted succesfully, otherwise false.</returns>
        public virtual bool Insert(TEntity entity)
        {
            using (var connector = new Connector(this.DataFile))
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
                    else if (propertyValue is Guid)
                    {
                        propertyValue = propertyValue.ToString();
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
        public virtual IEnumerable<TEntity> All()
        {
            using (var connector = new Connector(DataFile))
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
        public virtual TEntity GetById(object id)
        {
            using (var connector = new Connector(DataFile))
            {
                var properties = GetEntityPropertyNames();

                var columns = string.Join(",", properties.Select(property => string.Format("[{0}]", property)));

                var sql = string.Format(SELECT_BY_ID, columns, EntityType.Name);

                var entity = connector.SelectEntity<TEntity>(sql, new[] { 
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
        public virtual void Delete(object id)
        {
            using (var connector = new Connector(DataFile))
            {
                var sql = string.Format(DELETE, EntityType.Name);

                OnBeforeDelete(id);

                connector.Execute(sql, new[] { new SQLiteQueryParameter(":ID", id) });

                OnAfterDelete(id);
            }
        }

        #endregion
    }
}
