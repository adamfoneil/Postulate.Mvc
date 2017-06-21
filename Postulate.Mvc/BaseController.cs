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
using Postulate.Orm.Interfaces;
using System.Collections.Generic;

namespace Postulate.Mvc
{
    public abstract class BaseController<TDb, TKey, TProfile> : Controller 
        where TDb : SqlServerDb<TKey>, new() 
        where TProfile : Record<TKey>, IUserProfile
    {
        private SqlServerDb<TKey> _db = new TDb();
        private TProfile _profile = null;

        protected SqlServerDb<TKey> Db { get { return _db; } }
        protected TProfile CurrentUser { get { return _profile; } }

        /// <summary>
        /// SelectListQueries to execute when FillSelectLists is called
        /// </summary>        
        protected abstract IEnumerable<SelectListQuery> SelectListQueries();

        /// <summary>
        /// Parameters required by all queries in SelectListQueries()
        /// </summary>
        /// <returns></returns>
        protected virtual object SelectListParameters()
        {
            return null;
        }

        /// <summary>
        /// Indicates where to redirect if current user has no TProfile record
        /// </summary>
        protected abstract string ProfileUpdateUrl { get; }        

        protected override void Initialize(RequestContext requestContext)
        {            
            base.Initialize(requestContext);
            _db.UserName = User.Identity.Name;
            _profile = Db.FindUserProfile<TProfile>();            
        }        

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {            
            base.OnActionExecuting(filterContext);

            if (_profile == null)
            {
                // thanks to https://stackoverflow.com/questions/32925219/how-to-create-a-custom-attribute-that-will-redirect-to-login-if-it-returns-false
                filterContext.Result = new RedirectResult(ProfileUpdateUrl);
            }
        }

        /// <summary>
        /// Saves a record and returns true if successful. Otherwise, the error message is set in TempData
        /// </summary>
        protected bool SaveRecord<TRecord>(TRecord record) where TRecord : Record<TKey>
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
        protected bool DeleteRecord<TRecord>(TKey id) where TRecord : Record<TKey>
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
        /// Fills multiple SelectLists with a single server round trip
        /// </summary>
        protected void FillSelectLists(object record)
        {
            using (var cn = Db.GetConnection())
            {
                cn.Open();
                FillSelectLists(cn, record, SelectListQueries());
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
                FillSelectLists(cn, record, queries.Concat(SelectListQueries()));
            }
        }

        /// <summary>
        /// Fills multiple SelectLists with a single server round trip
        /// </summary>
        protected void FillSelectLists(IDbConnection connection, object record, IEnumerable<SelectListQuery> queries)
        {            
            if (!queries?.Any() ?? false) return;

            var props = record.GetType().GetProperties();

            var gridReader = connection.QueryMultiple(string.Join("\r\n", queries.Select(q => $"{q.Sql};")), SelectListParameters());

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

        private object GetSelectedValue(object record, PropertyInfo[] props, string valueProperty, out bool isDefaultValue)
        {
            var property = props.SingleOrDefault(pi => pi.Name.Equals(valueProperty));
            var result = property?.GetValue(record);

            // thanks to https://stackoverflow.com/questions/325426/programmatic-equivalent-of-defaulttype
            var defaultValue = (property?.PropertyType.IsValueType ?? false) ? Activator.CreateInstance(property.PropertyType) : null;
            isDefaultValue = (defaultValue?.Equals(result) ?? true);

            return result;
        }
    }
}
