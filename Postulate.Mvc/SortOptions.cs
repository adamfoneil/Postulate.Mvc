using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using static Postulate.Mvc.SortOptions;

namespace Postulate.Mvc
{
	/// <summary>
	/// Defines a list of insertable ORDER BY options for a SQL query without exposing your query to injection
	/// </summary>
	public class SortOptions : Dictionary<string, Item>
	{
		public SelectList ToSelectList(string selected = null)
		{
			return new SelectList(GetItems(selected), selected);
		}

		public IEnumerable<SelectListItem> GetItems(string selected = null)
		{
			return this.Select(kp => new SelectListItem() { Value = kp.Key, Text = kp.Value.Text, Selected = kp.Key.Equals(selected) });
		}

		public string GetSyntax(string key)
		{
			return this[key].Syntax;
		}

		public class Item
		{
			/// <summary>
			/// Description of option visible to user
			/// </summary>
			public string Text { get; set; }

			/// <summary>
			/// SQL syntax used in ORDER BY clause
			/// </summary>
			public string Syntax { get; set; }
		}
	}
}