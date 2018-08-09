using Postulate.Orm;
using Postulate.Orm.Interfaces;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Mvc;

namespace Postulate.Mvc
{
	/// <summary>
	/// Defines a query that returns items for a SelectList
	/// </summary>
	public abstract class SelectListQuery : Query<SelectListItem>
	{
		private readonly string _sql;
		private readonly string _valueProperty;

		/// <summary>
		/// Query must return columns Value and Text both strings
		/// </summary>
		/// <param name="sql">Text of the query</param>
		/// <param name="valueProperty">Name of property that stores the default value for the SelectList when rendered</param>
		public SelectListQuery(string sql, string valueProperty, ISqlDb db) : base(sql, db)
		{
			_sql = sql;
			_valueProperty = valueProperty;
		}

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

		/// <summary>
		/// Override this to modify the displayed list of items in some way that can't be easily represented in the original query
		/// </summary>
		protected virtual IEnumerable<SelectListItem> OnExecuted(IDbConnection connection, IEnumerable<SelectListItem> sourceItems)
		{
			return sourceItems;
		}

		public SelectList Execute(IDbConnection connection, object selectedValue = null)
		{
			var list = base.Execute(connection);
			return new SelectList(list, "Value", "Text", selectedValue);
		}

		public SelectList Execute(object selectedValue = null)
		{
			using (var cn = Db.GetConnection())
			{
				cn.Open();
				return Execute(cn, selectedValue);
			}
		}

		public SelectListItem QueryItem(IDbConnection connection, object selectedValue)
		{
			var dictionary = base.Execute(connection).ToDictionary(item => item.Value);
			return dictionary[selectedValue.ToString()];
		}

		public SelectListItem QueryItem(object selectedValue)
		{
			using (var cn = Db.GetConnection())
			{
				return QueryItem(cn, selectedValue);
			}
		}
	}
}