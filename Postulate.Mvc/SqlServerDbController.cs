using Postulate.Orm;
using System.Web.Mvc;
using System.Web.Routing;
using Postulate.Orm.Abstract;
using System;
using Postulate.Orm.Exceptions;
using System.Data;
using Dapper;
using System.Linq;
using System.Reflection;

namespace Postulate.Mvc
{
    public class SqlServerDbController<TDb, TKey> : Controller where TDb : SqlServerDb<TKey>, new()
    {
        protected SqlServerDb<TKey> _db = new TDb();

        protected override void Initialize(RequestContext requestContext)
        {            
            base.Initialize(requestContext);
            _db.UserName = User.Identity.Name;
        }

        protected bool SaveRecord<TRecord>(TRecord record) where TRecord : Record<TKey>
        {
            try
            {
                _db.Save(record);
                return true;
            }
            catch (Exception exc)
            {
                CaptureErrorMessage(exc);
                return false;
            }            
        }

        protected bool DeleteRecord<TRecord>(TKey id) where TRecord : Record<TKey>
        {
            try
            {
                _db.DeleteOne<TRecord>(id);
                return true;
            }
            catch (Exception exc)
            {
                CaptureErrorMessage(exc);
                return false;
            }            
        }

        protected void CaptureErrorMessage(Exception exc)
        {
            if (TempData.ContainsKey("error")) TempData.Remove("error");
            TempData.Add("error", exc.Message);

            SaveException se = exc as SaveException;
            if (se != null)
            {
                TempData.Add("command", se.CommandText);
                TempData.Add("record", se.Record);
            }
        }

        /// <summary>
        /// Fills multiple SelectLists with a single query round trip
        /// </summary>
        /// <typeparam name="TRecord"></typeparam>
        /// <param name="record"></param>
        /// <param name="parameters"></param>
        /// <param name="queries"></param>
        protected void FillSelectListsInner<TRecord>(TRecord record, object parameters, params SelectListQuery[] queries) where TRecord : Record<TKey>
        {
            using (var cn = _db.GetConnection())
            {
                cn.Open();
                FillSelectListsInner(cn, record, parameters, queries);
            }
        }

        protected void FillSelectListsInner<TRecord>(IDbConnection connection, TRecord record, object parameters, params SelectListQuery[] queries) where TRecord : Record<TKey>
        {            
            var props = typeof(TRecord).GetProperties();
            
            var gridReader = connection.QueryMultiple(string.Join("\r\n", queries.Select(q => $"{q.Sql};")), parameters);

            foreach (var q in queries)
            {
                var selectedValue = GetSelectedValue(record, props, q.ValueProperty);
                var listItems = gridReader.Read<SelectListItem>().Select(item => new SelectListItem() { Value = item.Value, Text = item.Text }).ToList();

                if (selectedValue != null && !listItems.Any(item => item.Value.Equals(selectedValue)))
                {
                    var missingItem = q.GetMissingItem(connection, selectedValue);
                    if (missingItem != null) listItems.Insert(0, missingItem);
                }

                ViewData.Add(q.ViewDataKey, new SelectList(listItems, "Value", "Text", selectedValue));
            }
        }

        private object GetSelectedValue<TRecord>(TRecord record, PropertyInfo[] props, string valueProperty) where TRecord : Record<TKey>
        {
            var property = props.SingleOrDefault(pi => pi.Name.Equals(valueProperty));
            return property?.GetValue(record);            
        }
    }
}
