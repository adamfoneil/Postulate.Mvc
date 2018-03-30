using Dapper;
using Postulate.Mvc.Abstract;
using Postulate.Mvc.Extensions;
using Postulate.Mvc.Interfaces;
using Postulate.Orm;
using Postulate.Orm.Abstract;
using Postulate.Orm.Exceptions;
using Postulate.Orm.Extensions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Web.Mvc;
using System.Web.Routing;

namespace Postulate.Mvc
{
	public abstract class ControllerBase<TDb, TKey> : Controller where TDb : SqlDb<TKey>, new()
	{
		private SqlDb<TKey> _db = new TDb();
		private bool _traceQueries = false;

		protected SqlDb<TKey> Db { get { return _db; } }

		/// <summary>
		/// SelectListQueries to execute when FillSelectLists is called
		/// </summary>
		protected virtual IEnumerable<SelectListQuery> SelectListQueries(object record = null) { return null; }

		/// <summary>
		/// Set this to cause exceptions in the controller to redirect to this view, as well as to cause <see cref="LogException(HandleErrorInfo)"/> to be called
		/// </summary>
		protected string ExceptionView { get; set; }

		/// <summary>
		/// Called when an exception occurs in the controller and <see cref="ExceptionView"/> is set
		/// </summary>
		protected virtual void LogException(HandleErrorInfo errorInfo)
		{
			// do nothing by default
		}

		protected override void Initialize(RequestContext requestContext)
		{
			base.Initialize(requestContext);
			_db.UserName = User.Identity.Name;
			Db.TraceQueries = TraceQueries(requestContext);
		}

		private bool TraceQueries(RequestContext requestContext)
		{
			try
			{				
				if (AllowQueryTrace(requestContext))
				{
					return (requestContext.HttpContext.Request.QueryString["tracequeries"].Equals("1"));
				}
				return false;
			}
			catch
			{
				// do nothing
			}
			return false;
		}

		/// <summary>
		/// Override this to implement a custom rule for determining when query tracing is allowed. By default, query tracing is not allowed
		/// </summary>		
		protected virtual bool AllowQueryTrace(RequestContext requestContext)
		{
			return false;
		}

		protected override void OnException(ExceptionContext filterContext)
		{
			base.OnException(filterContext);

			if (!string.IsNullOrEmpty(ExceptionView))
			{
				filterContext.ExceptionHandled = true;
				var model = new HandleErrorInfo(
					filterContext.Exception,
					filterContext.RequestContext.RouteData.Values["controller"].ToString(),
					filterContext.RequestContext.RouteData.Values["action"].ToString());

				LogException(model);

				filterContext.Result = new ViewResult()
				{
					ViewName = ExceptionView,
					ViewData = new ViewDataDictionary(model)
				};
			}
		}

		/// <summary>
		/// Updates a record and returns true if successful. Otherwise, the error message is set in TempData
		/// </summary>
		protected bool UpdateRecord<TRecord>(IDbConnection connection, TRecord record, params Expression<Func<TRecord, object>>[] setColumns) where TRecord : Record<TKey>, new()
		{
			try
			{
				Db.Update(connection, record, setColumns);
				return true;
			}
			catch (Exception exc)
			{
				CaptureErrorMessage(exc, record);
				return false;
			}
		}

		/// <summary>
		/// Updates a record and returns true if successful. Otherwise, the error message is set in TempData
		/// </summary>
		protected bool UpdateRecord<TRecord>(TRecord record, params Expression<Func<TRecord, object>>[] setColumns) where TRecord : Record<TKey>, new()
		{
			using (var cn = Db.GetConnection())
			{
				return UpdateRecord(cn, record, setColumns);
			}
		}

		/// <summary>
		/// Saves a record and returns true if successful. Otherwise, the error message is set in TempData
		/// </summary>
		protected bool SaveRecord<TRecord>(IDbConnection connection, TRecord record) where TRecord : Record<TKey>, new()
		{
			try
			{
				Db.Save(connection, record);
				return true;
			}
			catch (Exception exc)
			{
				CaptureErrorMessage(exc, record);
				return false;
			}
		}

		/// <summary>
		/// Saves a record and returns true if successful. Otherwise, the error message is set in TempData
		/// </summary>
		protected bool SaveRecord<TRecord>(TRecord record) where TRecord : Record<TKey>, new()
		{
			using (var cn = Db.GetConnection())
			{
				return SaveRecord(cn, record);
			}
		}

		/// <summary>
		/// Deletes a record and returns true if successful. Otherwise, the error message is set in TempData
		/// </summary>
		protected bool DeleteRecord<TRecord>(IDbConnection connection, TKey id) where TRecord : Record<TKey>, new()
		{
			try
			{
				Db.DeleteOne<TRecord>(connection, id);
				return true;
			}
			catch (Exception exc)
			{
				TRecord errorRecord = Db.Find<TRecord>(connection, id);
				CaptureErrorMessage(exc, errorRecord);
				return false;
			}
		}

		/// <summary>
		/// Deletes a record and returns true if successful. Otherwise, the error message is set in TempData
		/// </summary>
		protected bool DeleteRecord<TRecord>(TKey id) where TRecord : Record<TKey>, new()
		{
			using (var cn = Db.GetConnection())
			{
				return DeleteRecord<TRecord>(cn, id);
			}
		}

		private void CaptureErrorMessage<TRecord>(Exception exc, TRecord record) where TRecord : Record<TKey>
		{
			string message = record.GetErrorMessage(Db, exc.Message);

			TempData.RemoveAndAdd("error", message);

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

			// need to know if we're using the same query more than once -- those are cases where we have the same dropdown with different selected values
			var dupTypes = queries.GroupBy(q => q.GetType().Name).Where(grp => grp.Count() > 1).Select(grp => grp.Key);

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

				string viewDataKey = q.GetType().Name;

				// if we're using the same dropdown more than once with different selected value, then the view data key must include the specific value property
				if (dupTypes.Contains(viewDataKey)) viewDataKey = $"{viewDataKey}.{q.ValueProperty}";

				ViewData.Add(viewDataKey, new SelectList(listItems, "Value", "Text", selectedValue));
			}
		}

		private DynamicParameters CombineParameters(IEnumerable<SelectListQuery> queries, object record, PropertyInfo[] props)
		{
			var paramValues = queries
				.SelectMany(q => q.GetType().GetProperties()
				.Where(pi => pi.PropertyType.IsSimpleType() && pi.GetValue(q) != null)
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
				foreach (var pi in props.Where(pi => pi.PropertyType.IsSimpleType() && !paramValues.Any(p => p.Name.Equals(pi.Name))))
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

		private object GetSelectedValue(object record, PropertyInfo[] props, string valueProperty, out bool isDefaultValue)
		{
			var property = props.SingleOrDefault(pi => pi.Name.Equals(valueProperty));
			var result = property?.GetValue(record);

			if (property?.PropertyType.IsNullable() ?? false)
			{
				isDefaultValue = (result == null);
			}
			else
			{
				// thanks to https://stackoverflow.com/questions/325426/programmatic-equivalent-of-defaulttype
				var defaultValue = (property?.PropertyType.IsValueType ?? false) ? Activator.CreateInstance(property.PropertyType) : null;
				isDefaultValue = (defaultValue?.Equals(result) ?? true);
			}

			return result;
		}

		protected T LoadUserData<T>(T defaultInstance = null) where T : UserData, new()
		{
			return UserData.Load(Server, User.Identity.Name, defaultInstance);
		}

		protected override void OnActionExecuted(ActionExecutedContext filterContext)
		{
			base.OnActionExecuted(filterContext);

			var model = filterContext.Controller.ViewData.Model as IQueryTrace;
			if (model != null)
			{
				model.QueryTraceEnabled = Db.TraceQueries;
				model.QueryTraces = _db.QueryTraces;
			}
		}
	}
}