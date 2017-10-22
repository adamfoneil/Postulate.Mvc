using Dapper;
using Postulate.Orm;
using Postulate.Orm.Abstract;
using Postulate.Orm.Exceptions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Web.Mvc;
using System.Web.Routing;
using Postulate.Mvc.Extensions;
using Postulate.Mvc.Abstract;

namespace Postulate.Mvc
{
    public abstract class BaseController<TDb, TKey> : Controller where TDb : SqlServerDb<TKey>, new()        
    {
        private SqlServerDb<TKey> _db = new TDb();

        protected SqlServerDb<TKey> Db { get { return _db; } }

        /// <summary>
        /// SelectListQueries to execute when FillSelectLists is called
        /// </summary>        
        protected virtual IEnumerable<SelectListQuery> SelectListQueries(object record = null) { return null; }

        protected override void Initialize(RequestContext requestContext)
        {
            base.Initialize(requestContext);
            _db.UserName = User.Identity.Name;            
        }

        /// <summary>
        /// Updates a record and returns true if successful. Otherwise, the error message is set in TempData
        /// </summary>
        protected bool UpdateRecord<TRecord>(TRecord record, params Expression<Func<TRecord, object>>[] setColumns) where TRecord : Record<TKey>, new()
        {
            try
            {
                Db.Update(record, setColumns);
                return true;
            }
            catch (Exception exc)
            {
                CaptureErrorMessage(exc);
                return false;
            }
        }

        /// <summary>
        /// Saves a record and returns true if successful. Otherwise, the error message is set in TempData
        /// </summary>
        protected bool SaveRecord<TRecord>(TRecord record) where TRecord : Record<TKey>, new()
        {
            try
            {                
                Db.Save(record);
                return true;
            }
            catch (Exception exc)
            {
                CaptureErrorMessage(exc);
                return false;
            }
        }

        /// <summary>
        /// Deletes a record and returns true if successful. Otherwise, the error message is set in TempData
        /// </summary>
        protected bool DeleteRecord<TRecord>(TKey id) where TRecord : Record<TKey>, new()
        {
            try
            {
                Db.DeleteOne<TRecord>(id);
                return true;
            }
            catch (Exception exc)
            {
                CaptureErrorMessage(exc);
                return false;
            }
        }

        private void CaptureErrorMessage(Exception exc)
        {                        
            TempData.RemoveAndAdd("error", exc.Message);            

            SaveException se = exc as SaveException;
            if (se != null)
            {
                TempData.RemoveAndAdd("command", se.CommandText);                
                TempData.RemoveAndAdd("record", se.Record);
            }
        }

        /// <summary>
        /// Fills multiple SelectLists with a single server round trip
        /// </summary>
        protected void FillSelectLists(object record)
        {
            using (var cn = Db.GetConnection())
            {
                cn.Open();
                FillSelectLists(cn, record, SelectListQueries(record));
            }
        }

        /// <summary>
        /// Fills multiple SelectLists with a single server round trip
        /// </summary>
        protected void FillSelectLists(object record, params SelectListQuery[] queries)
        {
            using (var cn = Db.GetConnection())
            {
                cn.Open();
                var builtInQueries = SelectListQueries(record);
                FillSelectLists(cn, record, (builtInQueries != null) ?
                    queries.Concat(builtInQueries) : 
                    queries);
            }
        }

        /// <summary>
        /// Fills multiple SelectLists with a single server round trip
        /// </summary>
        protected void FillSelectLists(IDbConnection connection, object record, IEnumerable<SelectListQuery> queries)
        {
            if (!queries?.Any() ?? false) return;

            var props = record?.GetType().GetProperties();

            var gridReader = connection.QueryMultiple(string.Join("\r\n", queries.Select(q => $"{q.Sql};")), CombineParameters(queries, record, props));

            foreach (var q in queries)
            {
                bool isDefaultValue;
                var selectedValue = GetSelectedValue(record, props, q.ValueProperty, out isDefaultValue);
                var listItems = gridReader.Read<SelectListItem>().Select(item => new SelectListItem() { Value = item.Value, Text = item.Text }).ToList();

                if (!isDefaultValue && selectedValue != null && !listItems.Any(item => item.Value.ToString().Equals(selectedValue.ToString())))
                {
                    var missingItem = q.GetMissingItem(connection, selectedValue);
                    if (missingItem != null) listItems.Insert(0, missingItem);
                }

                ViewData.Add(q.ViewDataKey, new SelectList(listItems, "Value", "Text", selectedValue));
            }
        }

        private DynamicParameters CombineParameters(IEnumerable<SelectListQuery> queries, object record, PropertyInfo[] props)
        {
            var paramValues = queries.SelectMany(q => q.GetType().GetProperties()
                .Where(pi => pi.GetValue(q) != null)
                .Select(pi => new { Name = pi.Name, Value = pi.GetValue(q) })
                .GroupBy(pi => pi.Name)
                .Select(grp => new
                {
                    Name = grp.Key,
                    Value = grp.First().Value
                }));

            var dp = new DynamicParameters();
            foreach (var pv in paramValues) dp.Add(pv.Name, pv.Value);

            if (record != null)
            {
                foreach (var pi in props.Where(pi => IsSimpleType(pi.PropertyType)))
                {
                    var value = pi.GetValue(record);
                    if (value != null)
                    {
                        if (value is DateTime && value.Equals(default(DateTime))) continue;
                        dp.Add(pi.Name, value);
                    }
                }
            }
            
            return dp;
        }
        
        private static bool IsSimpleType(Type type)
        {
            // thanks to http://stackoverflow.com/questions/2442534/how-to-test-if-type-is-primitive
            return
                type.IsValueType ||
                type.IsPrimitive ||
                new Type[] {
                typeof(String),
                typeof(Decimal),
                typeof(DateTime),
                typeof(DateTimeOffset),
                typeof(TimeSpan),
                typeof(Guid)
            }.Contains(type) ||
                Convert.GetTypeCode(type) != TypeCode.Object;
        }

        private object GetSelectedValue(object record, PropertyInfo[] props, string valueProperty, out bool isDefaultValue)
        {
            if (record == null)
            {
                isDefaultValue = false;
                return null;
            }

            var property = props.SingleOrDefault(pi => pi.Name.Equals(valueProperty));
            var result = property?.GetValue(record);

            // thanks to https://stackoverflow.com/questions/325426/programmatic-equivalent-of-defaulttype
            var defaultValue = (property?.PropertyType.IsValueType ?? false) ? Activator.CreateInstance(property.PropertyType) : null;
            isDefaultValue = (defaultValue?.Equals(result) ?? true);

            return result;
        }

        protected T LoadUserData<T>() where T : UserData, new()
        {
            return UserData.Load<T>(Server, User.Identity.Name);
        }
    }
}
