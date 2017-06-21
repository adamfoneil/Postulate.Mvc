using Dapper;
using Postulate.Orm.Interfaces;
using Postulate.Sql.Abstract;
using System;
using System.Data;
using System.Web.Mvc;

namespace Postulate.Mvc
{
    /// <summary>
    /// Defines a query that returns items for a SelectList
    /// </summary>
    public abstract class SelectListQuery
    {
        private readonly string _sql;
        private readonly string _viewDataKey;
        private readonly string _valueProperty;

        /// <summary>
        /// Query must return columns Value and Text both strings
        /// </summary>        
        public SelectListQuery(string sql, string viewDataKey, string valueProperty)
        {
            _sql = sql;
            _viewDataKey = viewDataKey;
            _valueProperty = valueProperty;
        }

        public string Sql { get { return _sql; } }

        /// <summary>
        /// Key used in the ViewData dictionary within a Razor view
        /// </summary>
        public string ViewDataKey { get { return _viewDataKey; } }

        /// <summary>
        /// Name of property that stores the default value for the SelectList when rendered
        /// </summary>
        public string ValueProperty { get { return _valueProperty; } }

        /// <summary>
        /// Override this to enable SelectLists to retrieve a list item that an object references but is not already in the list, 
        /// such as an "inactive" item that's normally filtered out.
        /// </summary>
        public virtual SelectListItem GetMissingItem(IDbConnection connection, object id)
        {
            return null;
        }

        public SelectList Execute(IDb db, object parameters = null, object selectedValue = null)
        {
            using (var cn = db.GetConnection())
            {
                cn.Open();
                return Execute(cn, parameters, selectedValue);
            }
        }

        public SelectList Execute(IDbConnection connection, object parameters = null, object selectedValue = null)
        {
            var items = connection.Query<SelectListItem>(Sql, parameters);
            return new SelectList(items, "Value", "Text", selectedValue);
        }
    }
}
